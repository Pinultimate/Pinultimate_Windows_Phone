using Microsoft.Phone.Maps.Controls;
using System.Device.Location;
using System.Windows;

namespace Pinultimate_Windows_Phone.Data
{
    public class TrendMapViewModel
    {
        public Map trendMap { get; set; }
        public ClusterList clusterList { get; set; }

        public TrendMapViewModel(Map TrendMap)
        {
            trendMap = TrendMap;
            trendMap.ZoomLevelChanged += TrendMap_ZoomLevelChanged;
            trendMap.CenterChanged += TrendMap_CenterChanged;
            clusterList = new ClusterList();
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
    }
}
