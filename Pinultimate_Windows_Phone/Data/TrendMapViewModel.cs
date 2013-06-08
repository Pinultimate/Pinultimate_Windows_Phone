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
using Microsoft.Phone.Maps.Services;
using LinqToTwitter;
using System.Windows.Threading;

namespace Pinultimate_Windows_Phone.Data
{
    public class TrendMapViewModel
    {
        #region "instance variables"

        public ClusterList clusterList { get; set; }
        public List<ClusteringProcessor> processors { get; set; }
        public MainPage mainPage { get; set; }
        private AppSettings appSettings
        {
            get
            {
                return mainPage.appSettings;
            }
        }
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
        public TextBlock secondaryNotificationText
        {
            get
            {
                return mainPage.SecondaryNotificationText;
            }
            set
            {
                mainPage.SecondaryNotificationText = value;
            }
        }
        private Ellipse meIndicator { get; set; }

        #endregion

        #region "Me"
        public GeoTracker geoTracker { get; set; }
        public GeoCoordinate meLocation { get; set; }

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
        private readonly List<Cluster> EMPTY_CLUSTER_LIST = new List<Cluster>();

        private MapLayer meIndicatorLayer;
        private MapLayer clustersLayer;
        private MapLayer searchResultsLayer;

        public TrendMapViewModel(MainPage MainPage)
        {
            mainPage = MainPage;
            ShowNotification("TrendMap", "Preparing TrendMap for you...");
            geoTracker = new GeoTracker(mainPage);
            geoTracker.StartTracking();

            locationFetcher = new LocationFetcher(
               typeof(QueryResult<GridLocationData>),
               new Pinultimate_Windows_Phone.LocationFetcher.JSONLoadingStartedHandler(fetchClustersStartedCallback),
               new Pinultimate_Windows_Phone.LocationFetcher.JSONLoadingCompletionHandler(fetchClustersFinishedCallback),
               new Pinultimate_Windows_Phone.LocationFetcher.JSONLoadingErrorHandler(fetchClustersErrorCallback)
            );

            clusterList = new ClusterList();
            clusterList.ClustersChanged += UpdateMapWithNewClusters;
            InitializeMeIndicatorAndLayers();
            trendMap.CenterChanged += trendMap_Loaded;
        }

        private void trendMap_Loaded(object sender, MapCenterChangedEventArgs e)
        {
            InitiateNewQuery();
            trendMap.CenterChanged -= trendMap_Loaded;
        }

        #region "data callbacks"

        public void ShowNotification(String title, String text)
        {
            notificationPanel.Visibility = Visibility.Visible;
            notificationText.Visibility = Visibility.Visible;
            secondaryNotificationText.Visibility = Visibility.Visible;
            secondaryNotificationText.Text = title;
            notificationText.Text = text;

            applicationBarViewModel.DisableNextButton();
            applicationBarViewModel.DisablePrevButton();
        }

        public void HideNotification()
        {
            notificationText.Visibility = Visibility.Collapsed;
            notificationPanel.Visibility = Visibility.Collapsed;
            secondaryNotificationText.Visibility = Visibility.Collapsed;

            applicationBarViewModel.ConfigureTimelineButtonsOnCondition();
        }

        public void ChangeNotificationText(String title, String newMessage)
        {
            secondaryNotificationText.Text = title;
            notificationText.Text = newMessage;
        }

        public bool NotificationVisible()
        {
            return notificationPanel.Visibility == Visibility.Visible;
        }

        public void fetchClustersStartedCallback()
        {
            if (NotificationVisible())
            {
                ChangeNotificationText("Abandoned previous query", "Refetching social data around you...");
            }
            else
            {
                ShowNotification("Location Data", "Fetching social data around you...");
            }
        }

        public void fetchClustersFinishedCallback(Object resultObject)
        {
            QueryResult<GridLocationData> result = (QueryResult<GridLocationData>)resultObject;
            fetchingClusters.Hide();
            processors = ClusteringProcessor.GetClusteringProcessors(result);
            List<Cluster> clusters = null;
            if (processors.Count() > 0)
            {
                int currentTimeIndex = timelineViewModel.GetCurrentValue();
                clusters = processors[currentTimeIndex].Clusters();
            }
            else
            {
                clusters = EMPTY_CLUSTER_LIST;
            }
            clusterList.AddResults(clusters);
            HideNotification();

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
            Analytics.tap();
            if (appSettings.SoundSetting)
            {
                SoundEffects.SFxClusterTap.Play();
            }
            Cluster currentCluster = (Cluster)(sender as Border).DataContext;
            
            NavigationUtils.Navigate(mainPage.NavigationService, "/ClusterInformationPanorama.xaml", currentCluster);
        }

        private void QueryTwitter(Cluster cluster)
        {
            // lat lon and radius
            String geoQueryString = cluster.Latitude + "," + cluster.Longitude + "," + cluster.Radius;


            Console.WriteLine("00000000");
            var auth = new SingleUserAuthorizer
            {
                Credentials = new InMemoryCredentials
                {
                    ConsumerKey = "A7R0OmDF2kJvdIwHkYaSaA",
                    ConsumerSecret = "FDFfHcKyr2vxa33qMghDPETi2CfztsxC9LPVNyJe0",
                    OAuthToken = "1373732868-rJSi4IAzTh5ruvimgs4NxsrZUtOOgKT8LLceeU",
                    AccessToken = "LtPxpNbsiy5tWgMd1SfEaEvt0NPlkXvRH3ky1hkovw"
                }
            };

            Console.WriteLine("1111");
            var twitterCtx = new TwitterContext(auth, "https://api.twitter.com/", "https://search.twitter.com/");
            Console.WriteLine("22222");


            (from search in twitterCtx.Search
             where search.Type == SearchType.Search &&
                   search.Query == "LINQ to Twitter" &&
                   search.Count == 1
             select search)
                .MaterializedAsyncCallback(asyncResponse =>
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        if (asyncResponse.Status != TwitterErrorStatus.Success)
                        {
                            MessageBox.Show("Error: " + asyncResponse.Exception.Message);
                            return;
                        }
                        var srch = asyncResponse.State.SingleOrDefault();
                        Console.WriteLine("\nQuery: {0}\n", srch.SearchMetaData.Query);
                        Console.WriteLine("\nGeoCode: {0}\n", geoQueryString);
                        srch.Statuses.ForEach(entry =>
                            Console.WriteLine("Content: {0}\n", entry.Text));
                    })
                );

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
            BeginLoadingProgress("GPS", "Relocating your position...");
            meLocation = await geoTracker.GetCurrentLocation();
            if (meLocation == null)
            {
                MessageBox.Show("We weren't able to locate your position. (Have you enabled Location Services on your phone?)",
                  "Location", MessageBoxButton.OK);
            }
            else
            {
                meIndicatorOverlay.GeoCoordinate = meLocation;
                meIndicator.Fill = new SolidColorBrush(appSettings.ThemeColor);
                LocateMapToCoordinate(meLocation);
            }
            EndLoadingProgress();
        }

        private async void InitializeMeIndicatorAndLayers()
        {
            Task<GeoCoordinate> geoCoordinateTask = geoTracker.GetCurrentLocation();
            if (geoCoordinateTask != null)
            {
                meIndicatorLayer = new MapLayer();
                meIndicatorOverlay = new MapOverlay();
                meIndicator = new Ellipse();
                meIndicator.Fill = new SolidColorBrush(appSettings.ThemeColor);
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

                searchResultsLayer = new MapLayer();
                trendMap.Layers.Add(searchResultsLayer);

                meLocation = await geoCoordinateTask;

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
        public void BeginLoadingProgress(String title, String message)
        {
            ShowNotification(title, message);
            loadingProgress.Visibility = Visibility.Visible;
            trendMap.Opacity = 0.5;
            timelineViewModel.setOpacity(0.5);
            meIndicator.Opacity = 0.2;


            applicationBarViewModel.DisableMeButton();
            applicationBarViewModel.DisableReloadMenu();

        }

        public void EndLoadingProgress()
        {
            loadingProgress.Visibility = Visibility.Collapsed;
            trendMap.Visibility = Visibility.Visible;
            trendMap.Opacity = 1;
            timelineViewModel.setVisibility(Visibility.Visible);
            timelineViewModel.setOpacity(1);
            meIndicator.Visibility = Visibility.Visible;
            meIndicator.Opacity = 0.5;

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
                cluster.Timestamp = timelineViewModel.GetCurrentTime();
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

        public void InitiateNewQuery()
        {
            LocationRectangle boundingBox = GetBoundingBox();

            locationFetcher.FetchClusters(
                boundingBox.Northwest.Latitude, boundingBox.Northwest.Longitude,
                boundingBox.WidthInDegrees, boundingBox.HeightInDegrees,
                timelineViewModel.GetMinimumTime(), timelineViewModel.GetMaximumTime()
            );
        }

        public void selectClustersForGivenTime()
        {
            int currentTimeIndex = timelineViewModel.GetCurrentValue();
            List<Cluster> clusters = processors[currentTimeIndex].Clusters();
            clusterList.AddResults(clusters);
        }

        public void AddSearchResultsToMap(IList<MapLocation> results)
        {
            ClearSearchResultsFromMap();
            // Add all results to MyCoordinates for drawing the map markers
            for (int i = 0; i < results.Count; i++)
            {
                DrawMapMarker(results[i].GeoCoordinate, Colors.Red, searchResultsLayer);
            }
        }

        public void ClearSearchResultsFromMap()
        {
            searchResultsLayer.Clear();
        }

        public bool SearchResultsPresent()
        {
            return searchResultsLayer.Count > 0;
        }

        private void DrawMapMarker(GeoCoordinate coordinate, Color color, MapLayer mapLayer)
        {
            // Create a map marker
            Polygon polygon = new Polygon();
            polygon.Points.Add(new Point(0, 0));
            polygon.Points.Add(new Point(0, 75));
            polygon.Points.Add(new Point(25, 0));
            polygon.Fill = new SolidColorBrush(color);

            // Enable marker to be tapped for location information
            polygon.Tag = new GeoCoordinate(coordinate.Latitude, coordinate.Longitude);
            //polygon.MouseLeftButtonUp += new MouseButtonEventHandler(Marker_Click);

            // Create a MapOverlay and add marker
            MapOverlay overlay = new MapOverlay();
            overlay.Content = polygon;
            overlay.GeoCoordinate = new GeoCoordinate(coordinate.Latitude, coordinate.Longitude);
            overlay.PositionOrigin = new Point(0.0, 1.0);
            mapLayer.Add(overlay);
        }
    }
}
