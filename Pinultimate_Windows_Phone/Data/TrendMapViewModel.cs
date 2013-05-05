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

namespace Pinultimate_Windows_Phone.Data
{
    public class TrendMapViewModel
    {
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

        #region "Me"
        public GeoTracker geoTracker { get; set; }
        public GeoCoordinate meLocation { get; set; }
        //public Ellipse meIndicator { get; set; }
        public MapOverlay meIndicatorOverlay { get; set; }
        #endregion

        private readonly LocationFetcher locationFetcher = new LocationFetcher();
        private MapLayer meIndicatorLayer;

        public TrendMapViewModel(MainPage MainPage)
        {
            mainPage = MainPage;
            trendMap.ZoomLevelChanged += TrendMap_ZoomLevelChanged;
            trendMap.CenterChanged += TrendMap_CenterChanged;
            geoTracker = new GeoTracker(mainPage);
            geoTracker.StartTracking();
            clusterList = new ClusterList();
            clusterList.ClustersChanged += UpdateMapWithNewClusters;
            InitializeMeIndicator();
        }

        #region "data callbacks"

        public void fetchClustersCallback(QueryResult<GridLocationData> result)
        {
            processors = ClusteringProcessor.GetClusteringProcessors(result);
            // TODO: we only get the first one for now, we'll get the rest later
            // TODO: make it asynchronous
            if (processors.Count() > 0)
            {
                List<Cluster> clusters = processors[processors.Count - 1].Clusters();
                clusterList.AddResults(clusters);
            }
            else
            {
                MessageBox.Show("No clusters could be found",
                  "Location",
                  MessageBoxButton.OK);
            }
            
        }

        #endregion

        #region "map UI callbacks"

        private void TrendMap_CenterChanged(object sender, MapCenterChangedEventArgs e)
        {
            LocationRectangle boundingBox = GetBoundingBox();

            locationFetcher.FetchClusters(new Pinultimate_Windows_Phone.LocationFetcher.JSONLoadingCompletionHandler(fetchClustersCallback),
    boundingBox.Northwest.Latitude, boundingBox.Northwest.Longitude, boundingBox.WidthInDegrees, boundingBox.HeightInDegrees);
        }

        private void TrendMap_ZoomLevelChanged(object sender, MapZoomLevelChangedEventArgs e)
        {
            LocationRectangle boundingBox = GetBoundingBox();
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
            meLocation = await geoTracker.GetCurrentLocation();
            meIndicatorOverlay.GeoCoordinate = meLocation;
        }

        public void LocateMapToMe()
        {
            LocateMapToCoordinate(meLocation);
        }

        private async void InitializeMeIndicator()
        {
            Task<GeoCoordinate> geoCoordinateTask = geoTracker.GetCurrentLocation();
            if (geoCoordinateTask != null)
            {
                meIndicatorLayer = new MapLayer();
                meIndicatorOverlay = new MapOverlay();
                Ellipse meIndicator = new Ellipse();
                meIndicator.Fill = new SolidColorBrush(Colors.Blue);
                meIndicator.Height = 10;
                meIndicator.Width = 10;
                meIndicator.Opacity = 50;
                meIndicatorOverlay.Content = meIndicator;
                meIndicatorOverlay.PositionOrigin = new Point(0.5, 0.5);
                meIndicatorLayer.Add(meIndicatorOverlay);
                meLocation = await geoTracker.GetCurrentLocation();
                meIndicatorOverlay.GeoCoordinate = meLocation;
                trendMap.Layers.Add(meIndicatorLayer);
                LocateMapToMe();
            }
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
            
            if (clusterList.Count == 0)
            {
                ClearMap();
                return;
            }
            Debug.WriteLine("Drawing Clusters");
            foreach (Cluster cluster in clusterList)
            {
                DrawCluster(cluster);
            }
        }

        private void ClearMap()
        {
            trendMap.Layers.Clear();
            trendMap.Layers.Add(meIndicatorLayer);
        }

        private void DrawCluster(Cluster cluster)
        {
            trendMap.Layers.Add(TrendMapDrawingUtils.CreateMapLayerForCluster(cluster));
        }
    }
}
