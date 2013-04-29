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

namespace Pinultimate_Windows_Phone.Data
{
    public class TrendMapViewModel
    {
        public Map trendMap { get; set; }
        public ClusterList clusterList { get; set; }
        public MainPage mainPage { get; set; }

        #region "Me"
        public GeoTracker geoTracker { get; set; }
        public GeoCoordinate meLocation { get; set; }
        //public Ellipse meIndicator { get; set; }
        public MapOverlay meIndicatorOverlay { get; set; }
        #endregion

        private readonly LocationFetcher locationFetcher = new LocationFetcher();

        public TrendMapViewModel(Map TrendMap, MainPage MainPage)
        {
            mainPage = MainPage;
            trendMap = TrendMap;
            trendMap.ZoomLevelChanged += TrendMap_ZoomLevelChanged;
            trendMap.CenterChanged += TrendMap_CenterChanged;
            geoTracker = new GeoTracker(mainPage);
            geoTracker.StartTracking();
            InitializeMeIndicator();
            clusterList = new ClusterList();
            clusterList.ClustersChanged += UpdateMapWithNewClusters;
        }

        #region "callbacks"
        private void TrendMap_CenterChanged(object sender, MapCenterChangedEventArgs e)
        {
            LocationRectangle boundingBox = getBoundingBox();
        }

        private void TrendMap_ZoomLevelChanged(object sender, MapZoomLevelChangedEventArgs e)
        {
            LocationRectangle boundingBox = getBoundingBox();
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
                MapLayer meIndicatorLayer = new MapLayer();
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

        private LocationRectangle getBoundingBox()
        {
            GeoCoordinate Point1 = trendMap.ConvertViewportPointToGeoCoordinate(new Point(0, 0));
            GeoCoordinate Point2 = trendMap.ConvertViewportPointToGeoCoordinate(new Point(trendMap.ActualHeight, trendMap.ActualWidth));
            return new LocationRectangle(Point1, Point2);
        }

        private void UpdateMapWithNewClusters(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (Cluster cluster in clusterList)
            {
                DrawCluster(cluster);
            }
        }

        private void DrawCluster(Cluster cluster)
        {
            trendMap.Layers.Add(TrendMapDrawingUtils.CreateMapLayerForCluster(cluster));
        }

        public void FetchLocations(double latitude, double longitude, double latrange, double lonrange, double resolution)
        {
            Cluster[] results = locationFetcher.FetchClusters(latitude, longitude, latrange, lonrange, resolution);
            clusterList.AddResults(results);
        }

    }
}
