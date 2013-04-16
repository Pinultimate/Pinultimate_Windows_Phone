using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Testing;
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
using System.Diagnostics;
using Windows.Devices.Geolocation;
using System.IO.IsolatedStorage;

namespace Pinultimate_Windows_Phone
{
    public partial class MainPage : PhoneApplicationPage
    {
        private GeoTracker geoTracker;
        private ApplicationBar appBar;

        public MainPage()
        {
            InitializeComponent();
            buildApplicationBar();
            this.geoTracker = new GeoTracker(this);
            this.Content = UnitTestSystem.CreateTestPage();
        }

        public void updateAppBar(String status)
        {
             Dispatcher.BeginInvoke(new Action(() =>
            {
                
                
            }));
        }

        public void updateMap(double Latitude, double Longitude)
        {
            Dispatcher.BeginInvoke(() =>
            {
                //LatitudeTextBlock.Text = args.Position.Coordinate.Latitude.ToString("0.00");
                //LongitudeTextBlock.Text = args.Position.Coordinate.Longitude.ToString("0.00");
            });
        }

        private void buildApplicationBar()
        {
            this.appBar = new ApplicationBar();
            this.appBar.IsMenuEnabled = false;
            this.appBar.IsVisible = true;

            ApplicationBarIconButton settings = new ApplicationBarIconButton();
            settings.IconUri = new Uri("/Images/settings.png", UriKind.Relative);
            settings.Text = "settings";
            this.appBar.Buttons.Add(settings);
            settings.Click += new EventHandler(Settings_Click);

            ApplicationBarIconButton tracker = new ApplicationBarIconButton();
            tracker.IconUri = new Uri("/Images/play.png", UriKind.Relative);
            tracker.Text = "track";
            this.appBar.Buttons.Add(tracker);
            tracker.Click += new EventHandler(TrackLocation_Click);
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Settings button works!");
            //Do work for your application here.
            throw new NotImplementedException();
        }

        private void TrackLocation_Click(object sender, EventArgs e)
        {
            if ((bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"] != true)
            {
                // The user has opted out of Location.
                return;
            }

            ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[0];

            if (!this.geoTracker.IsTracking())
            {
                Debug.Assert(btn.Text == "track");
                geoTracker.StartTracking();
                btn.Text = "don't track";
                btn.IconUri = new Uri("/Images/stop.png", UriKind.Relative);
            }
            else
            {
                Debug.Assert(btn.Text == "don't track");
                geoTracker.StopTracking();
                btn.Text = "track";
                btn.IconUri = new Uri("/Images/start.png", UriKind.Relative);
            }
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
            Task<GeoCoordinate> geoCoordinateTask = geoTracker.GetCurrentLocation();
            if (geoCoordinateTask != null)
            {
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
        }

        /* This function is a place holder for a much more advanced function that will
         * correctly handle data being received from the server.  This function will be
         * called when data has been received and successfully parsed.*/
        private void checkinHandler(QueryResult<GridLocationData> result)
        {

            Debug.WriteLine("\nNumber Of Response Objects: {0}",result.ResponseData.Count());
            foreach (ResponseData<GridLocationData> timePeriodData in result.ResponseData) 
            {
                Debug.WriteLine("\nTimeStamp: {0}", timePeriodData.RawTimestamp);
                foreach (GridLocationData gridLocDataPoint in timePeriodData.LocationData)
                {
                    Debug.WriteLine("\nLatitude: {0} , Longitude: {1}, Count: {2}", gridLocDataPoint.Latitude, gridLocDataPoint.Longitude, gridLocDataPoint.Count);
                }
            }
        }

        private void testAPI()
        {
            LocationFetcher locFetcher = new LocationFetcher();

            string gridQuery = QueryURL.CreateGridQuery(37.0, -122.0, 0.5, 0.5, 5);
            locFetcher.JSONResponseForURL(gridQuery,checkinHandler);
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

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("LocationConsent"))
            {
                // User has opted in or out of Location
                return;
            }
            else
            {
                MessageBoxResult result =
                    MessageBox.Show("This app accesses your phone's location. Is that ok?",
                    "Location",
                    MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = true;
                    IsolatedStorageSettings.ApplicationSettings["LocationTimeInterval"] = "1 min";
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = false;
                }

                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }
    }
}