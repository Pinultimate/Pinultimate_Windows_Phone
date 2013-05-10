using Microsoft.Phone.Maps.Controls;
using System.Device.Location;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Threading;
using System.Diagnostics;
using Coding4Fun.Toolkit.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Shell;

namespace Pinultimate_Windows_Phone.Data
{
    public class TrendMapViewModel
    {
        #region "instance variables"

        public ClusterList clusterList { get; set; }
        public List<ClusteringProcessor> processors { get; set; }
        public MainPage mainPage { get; set; }
        public Map trendMap
        {
            get
            {
                return mainPage.TrendMap;
            }
            set
            {
                mainPage.TrendMap = value;
            }
        }
        public ProgressBar loadingProgress
        {
            get
            {
                return mainPage.LoadingProgress;
            }
            set
            {
                mainPage.LoadingProgress = value;
            }
        }
        private TimelineViewModel timelineViewModel
        {
            get
            {
                return mainPage.timelineViewModel;
            }
        }
        private ApplicationBarViewModel applicationBarViewModel
        {
            get
            {
                return mainPage.applicationBarViewModel;
            }
        }
        public Canvas notificationPanel
        {
            get
            {
                return mainPage.NotificationPanel;
            }
            set
            {
                mainPage.NotificationPanel = value;
            }
        }
        public TextBlock notificationText
        {
            get
            {
                return mainPage.NotificationText;
            }
            set
            {
                mainPage.NotificationText = value;
            }
        }
        private Ellipse meIndicator { get; set; }

        #endregion

        #region "Me"
        public GeoTracker geoTracker { get; set; }
        public GeoCoordinate meLocation { get; set; }
        //public Ellipse meIndicator { get; set; }
        public MapOverlay meIndicatorOverlay { get; set; }
        #endregion

        #region "Toast Prompts"

        private readonly ToastPrompt fetchingClusters = new ToastPrompt
            {
                Title = "Gathering clusters...",
                ImageSource = new BitmapImage(new Uri("../../ApplicationIcon.png", UriKind.RelativeOrAbsolute)),
                //MillisecondsUntilHidden = 2000
            };

        private readonly ToastPrompt noClustersFound = new ToastPrompt
            {
                Title = "No clusters found",
                ImageSource = new BitmapImage(new Uri("../../ApplicationIcon.png", UriKind.RelativeOrAbsolute)),
                MillisecondsUntilHidden = 2000
            };
        #endregion

        private readonly LocationFetcher locationFetcher; 
        private MapLayer meIndicatorLayer;
        private MapLayer clustersLayer;

        public TrendMapViewModel(MainPage MainPage)
        {
            mainPage = MainPage;
            ShowNotification("Preparing TrendMap for you...");
            geoTracker = new GeoTracker(mainPage);
            geoTracker.StartTracking();

            locationFetcher = new LocationFetcher(
               new Pinultimate_Windows_Phone.LocationFetcher.JSONLoadingStartedHandler(fetchClustersStartedCallback),
               new Pinultimate_Windows_Phone.LocationFetcher.JSONLoadingCompletionHandler(fetchClustersFinishedCallback),
               new Pinultimate_Windows_Phone.LocationFetcher.JSONLoadingErrorHandler(fetchClustersErrorCallback)
            );

            clusterList = new ClusterList();
            clusterList.ClustersChanged += UpdateMapWithNewClusters;
            InitializeMeIndicatorAndClustersLayer();
        }

        #region "data callbacks"

        public void ShowNotification(String text)
        {
            notificationPanel.Visibility = Visibility.Visible;
            notificationText.Visibility = Visibility.Visible;
            notificationText.Text = text;
        }

        public void HideNotification()
        {
            notificationText.Visibility = Visibility.Collapsed;
            notificationPanel.Visibility = Visibility.Collapsed;
        }

        public void ChangeNotificationText(String newMessage)
        {
            notificationText.Text = newMessage;
        }

        public bool NotificationVisible()
        {
            return notificationPanel.Visibility == Visibility.Visible;
        }

        public void fetchClustersStartedCallback ()
        {
            //fetchingClusters.Show();
            if (NotificationVisible())
            {
                ChangeNotificationText("Abandoning previous search. Refetching social data around you...");
            }
            else
            {
                ShowNotification("Fetching social data around you...");
            }
        }

        public void fetchClustersFinishedCallback(QueryResult<GridLocationData> result)
        {
            fetchingClusters.Hide();
            processors = ClusteringProcessor.GetClusteringProcessors(result);
            // TODO: we only get the first one for now, we'll get the rest later
            // TODO: make it asynchronous
            if (processors.Count() > 0)
            {
                List<Cluster> clusters = processors[processors.Count - 1].Clusters();
                clusterList.AddResults(clusters);
            }
            HideNotification();
            //else
            //{
            //    noClustersFound.Show();
            //}
            
        }

        private void fetchClustersErrorCallback()
        {
            MessageBox.Show("We weren't able to retrieve any data from the server. Check to see if you have Internet access.",
                  "Location",
                  MessageBoxButton.OK);
        }

        #endregion

        #region "map UI callbacks"
        public class QueryLocationParams
        {
            public double Resolution { get; set; }
            public double Longitude { get; set; }
            public double Latitude { get; set; }
            public double LonRange { get; set; }
            public double LatRange { get; set; }

            public QueryLocationParams(LocationRectangle boundingBox, int numGrids)
            {
                Longitude = boundingBox.Center.Longitude;
                Latitude = boundingBox.Center.Latitude;
                LonRange = boundingBox.WidthInDegrees;
                LatRange = boundingBox.HeightInDegrees;
                // ensure that the Longitude-wise there are numGrids' grids
                Resolution = boundingBox.WidthInDegrees / numGrids;
            }
        }

        private void TapOnCluster(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Cluster currentCluster = (Cluster) (sender as Border).DataContext;
            NavigationUtils.Navigate(mainPage.NavigationService, "/ClusterInformationPanorama.xaml", currentCluster);
        }

        #endregion

        #region "Map Center Change Functions"

        public void LocateMapToCoordinate(GeoCoordinate coordinate)
        {
            trendMap.Center = coordinate;
        }

        #endregion

        #region "Me Indicator"

        public async void RelocateAndRedrawMe()
        {
            BeginLoadingProgress("Relocating your position...");
            meLocation = await geoTracker.GetCurrentLocation();
            if (meLocation == null)
            {
                MessageBox.Show("We weren't able to locate your position. (Have you enabled Location Services on your phone?)",
                  "Location", MessageBoxButton.OK);
            }
            else
            {
                meIndicatorOverlay.GeoCoordinate = meLocation;
                LocateMapToCoordinate(meLocation);
            }
            EndLoadingProgress();
        }

        private async void InitializeMeIndicatorAndClustersLayer()
        {
            Task<GeoCoordinate> geoCoordinateTask = geoTracker.GetCurrentLocation();
            if (geoCoordinateTask != null)
            {
                meIndicatorLayer = new MapLayer();
                meIndicatorOverlay = new MapOverlay();
                meIndicator = new Ellipse();
                meIndicator.Fill = new SolidColorBrush(Colors.Blue);
                meIndicator.Height = 10;
                meIndicator.Width = 10;
                meIndicator.Opacity = 0.5;
                meIndicator.Visibility = Visibility.Collapsed;
                meIndicatorOverlay.Content = meIndicator;
                meIndicatorOverlay.PositionOrigin = new Point(0.5, 0.5);
                meIndicatorLayer.Add(meIndicatorOverlay);
                trendMap.Layers.Add(meIndicatorLayer);

                clustersLayer = new MapLayer();
                trendMap.Layers.Add(clustersLayer);

                meLocation = await geoCoordinateTask;

                //trendMap.ManipulationCompleted += trendMap_ManipulationCompleted;
                //trendMap.ManipulationStarted += trendMap_ManipulationStarted;
                //trendMap.ManipulationDelta += trendMap_ManipulationDelta;
                
                if (meLocation == null)
                {
                    MessageBox.Show("We weren't able to locate your position. (Have you enabled Location Services on your phone?)",
                      "Location", MessageBoxButton.OK);
                }
                else
                {
                    // are both necessary?
                    meIndicatorOverlay.GeoCoordinate = meLocation;
                    LocateMapToCoordinate(meLocation);
                }
                EndLoadingProgress();
            }
        }

        void trendMap_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            Debug.WriteLine("Manipulation Delta");
        }

        void trendMap_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            Debug.WriteLine("Manipulation Started");
        }

        void trendMap_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            Debug.WriteLine("Manipulation Completed");
        }

        #endregion

        #region "loading progress bar"
        private void BeginLoadingProgress(String message)
        {
            ShowNotification(message);
            loadingProgress.Visibility = Visibility.Visible;
            trendMap.Opacity = 0.5;
            timelineViewModel.setOpacity(0.5);
            meIndicator.Opacity = 0.2;

            applicationBarViewModel.DisableNextButton();
            applicationBarViewModel.DisablePrevButton();
            applicationBarViewModel.DisableMeButton();
            applicationBarViewModel.DisableReloadMenu();

        }

        private void EndLoadingProgress()
        {
            loadingProgress.Visibility = Visibility.Collapsed;
            trendMap.Visibility = Visibility.Visible;
            trendMap.Opacity = 1;
            timelineViewModel.setVisibility(Visibility.Visible);
            timelineViewModel.setOpacity(1);
            meIndicator.Visibility = Visibility.Visible;
            meIndicator.Opacity = 0.5;

            applicationBarViewModel.ConfigureTimelineButtonsOnCondition();
            applicationBarViewModel.EnableMeButton();
            applicationBarViewModel.EnableReloadMenu();

            HideNotification();
        }
        #endregion

        private LocationRectangle GetBoundingBox()
        {
            TimeSpan waitTime = new TimeSpan(0, 0, 0, 0, 1);
            GeoCoordinate Point1 = trendMap.ConvertViewportPointToGeoCoordinate(new Point(0, 0));
            //GeoCoordinate Point2 = trendMap.ConvertViewportPointToGeoCoordinate(new Point(trendMap.ActualHeight, trendMap.ActualWidth));
            GeoCoordinate Point2 = trendMap.ConvertViewportPointToGeoCoordinate(new Point(696, 480));
            while (Point1 == null || Point2 == null)
            {
                Thread.Sleep(waitTime);
            }
            return new LocationRectangle(Point1, Point2);
        }

        private void UpdateMapWithNewClusters(object sender, NotifyCollectionChangedEventArgs e)
        {

            ClearClustersFromMap();
            if (clusterList.Count == 0) return;
            Debug.WriteLine("Drawing Clusters");
            foreach (Cluster cluster in clusterList)
            {
                DrawCluster(cluster);
            }
        }

        private void ClearClustersFromMap()
        {
            clustersLayer.Clear();
        }

        private void DrawCluster(Cluster cluster)
        {
            clustersLayer.Add(TrendMapDrawingUtils.CreateMapLayerForCluster(cluster, TapOnCluster));
        }

        public void cancelCurrentQuery()
        {
            locationFetcher.cancelWebRequest();
        }

        public void initiateNewQuery()
        {
            LocationRectangle boundingBox = GetBoundingBox();

            locationFetcher.FetchClusters(
                boundingBox.Northwest.Latitude, boundingBox.Northwest.Longitude,
                boundingBox.WidthInDegrees, boundingBox.HeightInDegrees
            );
        }
    }
}
