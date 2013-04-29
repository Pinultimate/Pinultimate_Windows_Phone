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

namespace Pinultimate_Windows_Phone.Data
{
    public class TrendMapViewModel
    {
        public Map trendMap { get; set; }
        public ClusterList clusterList { get; set; }
        public GeoTracker geoTracker { get; set; }
        public MainPage mainPage { get; set; }

        private readonly LocationFetcher locationFetcher = new LocationFetcher();

        public TrendMapViewModel(Map TrendMap, MainPage MainPage)
        {
            mainPage = MainPage;
            trendMap = TrendMap;
            trendMap.ZoomLevelChanged += TrendMap_ZoomLevelChanged;
            trendMap.CenterChanged += TrendMap_CenterChanged;
            geoTracker = new GeoTracker(this.mainPage);
            clusterList = new ClusterList();
            clusterList.ClustersChanged += UpdateMapWithNewClusters;
        }

        private void TrendMap_CenterChanged(object sender, MapCenterChangedEventArgs e)
        {
            LocationRectangle boundingBox = getBoundingBox();
        }

        private void TrendMap_ZoomLevelChanged(object sender, MapZoomLevelChangedEventArgs e)
        {
            LocationRectangle boundingBox = getBoundingBox();
        }

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
