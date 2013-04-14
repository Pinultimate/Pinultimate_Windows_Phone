using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Shell;
using Pinultimate_Windows_Phone.Resources;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.Devices.Geolocation;

namespace Pinultimate_Windows_Phone
{
    public partial class MainPage : PhoneApplicationPage
    {
        private async Task<GeoCoordinate> getCurrentLocation()
        {
            // Get current location
            Geolocator locator = new Geolocator();
            Geoposition position = await locator.GetGeopositionAsync();
            Geocoordinate coordinate = position.Coordinate;
            return CoordinateConverter.ConvertGeocoordinate(coordinate);
        }

        private Ellipse createCircleForCurrentLocation()
        {
            Ellipse toReturn = new Ellipse();
            toReturn.Fill = new SolidColorBrush(Colors.Blue);
            toReturn.Height = 20;
            toReturn.Width = 20;
            toReturn.Opacity = 50;
            return toReturn;
        }

        private MapOverlay createMapOverlay(Ellipse circle)
        {
            MapOverlay toReturn = new MapOverlay();
            toReturn.Content = circle;
            toReturn.PositionOrigin = new Point(0.5, 0.5);
            return toReturn;
        }

        private async void showCurrentLocationOnMap()
        {
            // Get current location
            Task<GeoCoordinate> geoCoordinateTask = getCurrentLocation();
            

            // Create a small circle to mark the current location.
            Ellipse circle = createCircleForCurrentLocation();

            // Create a MapOverlay to contain the circle.
            MapOverlay overlay = createMapOverlay(circle);

            // Create a MapLayer to contain the MapOverlay.
            MapLayer currentLocationLayer = new MapLayer();
            currentLocationLayer.Add(overlay);

            // Add the MapLayer to the Map
            GeoCoordinate geoCoordinate = await geoCoordinateTask;
            PinultimateMap.Center = geoCoordinate;
            overlay.GeoCoordinate = geoCoordinate;
            PinultimateMap.Layers.Add(currentLocationLayer);
        }

        private void testAPI()
        {
            LocationFetcher locFetcher = new LocationFetcher();
            locFetcher.JSONResponseForURL("http://www.google.com");
        }

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            showCurrentLocationOnMap();
            testAPI();
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

           

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}