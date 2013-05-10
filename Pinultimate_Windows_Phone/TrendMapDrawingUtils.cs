using Microsoft.Phone.Maps.Controls;
using Pinultimate_Windows_Phone.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Pinultimate_Windows_Phone
{
    public class TrendMapDrawingUtils
    {

        private const int RADIUS_SCALE = 100;

        public static MapOverlay CreateMapLayerForCluster(Cluster cluster, EventHandler<GestureEventArgs> circle_Tap)
        {
            // Create a small circle to mark the current location.
            // Create a MapOverlay to contain the circle.
            Border circle = CreateCircleForMapDisplay(cluster.Radius, cluster.Count);
            circle.DataContext = cluster;
            circle.Tap += circle_Tap;
            MapOverlay overlay = CreateMapOverlay(circle);
            overlay.GeoCoordinate = cluster.GeoCoordinate;
            return overlay;
        }

        private static Border CreateCircleForMapDisplay(double radius, int count)
        {
            // Text label for circle, count number
            TextBlock countLabel = new TextBlock();
            countLabel.Text = (count).ToString();
            countLabel.TextAlignment = TextAlignment.Center;
            countLabel.VerticalAlignment = VerticalAlignment.Center;
            countLabel.Foreground = new SolidColorBrush(Colors.White);

            // Create "circle" that will contain circle
            Border border = new Border();
            double size = radius * RADIUS_SCALE;
            border.CornerRadius =  new CornerRadius(size / 2);
            //Ellipse toReturn = new Ellipse();
            border.Background = new SolidColorBrush(Colors.Green);
            border.BorderBrush = new SolidColorBrush(Colors.Black);
            border.BorderThickness = new Thickness(1);
            border.Height = size;
            border.Width = size;
            border.Opacity = 0.8;
            border.Child = countLabel;

            return border;
        }

        private static MapOverlay CreateMapOverlay(Border circle)
        {
            MapOverlay toReturn = new MapOverlay();
            toReturn.Content = circle;
            toReturn.PositionOrigin = new Point(0.5, 0.5);
            return toReturn;
        }
    }
}
