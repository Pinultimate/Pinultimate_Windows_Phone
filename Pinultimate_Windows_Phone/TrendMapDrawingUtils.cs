using Microsoft.Phone.Maps.Controls;
using Pinultimate_Windows_Phone.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Pinultimate_Windows_Phone
{
    class TrendMapDrawingUtils
    {
        public static MapLayer CreateMapLayerForCluster(Cluster cluster)
        {
            // Create a small circle to mark the current location.
            // Create a MapOverlay to contain the circle.
            MapOverlay overlay = CreateMapOverlay(CreateCircleForMapDisplay(cluster.Radius, cluster.Count));

            // Create a MapLayer to contain the MapOverlay.
            MapLayer clusterLayer = new MapLayer();
            clusterLayer.Add(overlay);

            overlay.GeoCoordinate = cluster.GeoCoordinate;
            return clusterLayer;
        }

        private static Ellipse CreateCircleForMapDisplay(double radius, int count)
        {
            // TODO add count as part of text
            Ellipse toReturn = new Ellipse();
            toReturn.Fill = new SolidColorBrush(Colors.Blue);
            toReturn.Height = radius;
            toReturn.Width = radius;
            toReturn.Opacity = 50;
            return toReturn;
        }

        private static MapOverlay CreateMapOverlay(Ellipse circle)
        {
            MapOverlay toReturn = new MapOverlay();
            toReturn.Content = circle;
            toReturn.PositionOrigin = new Point(0.5, 0.5);
            return toReturn;
        }
    }
}
