using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Shell;
using Pinultimate_Windows_Phone.Data;
using System;
using System.Device.Location;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pinultimate_Windows_Phone
{
    public partial class MainPage : PhoneApplicationPage
    {
        
        private DateTime currentTimestamp { get; set; }

        public AppSettings appSettings { get; set; }
        public ApplicationBarViewModel applicationBarViewModel { get; set; }
        public TrendMapViewModel trendMapViewModel { get; set; }
        public SearchBarViewModel searchBarViewModel { get; set; }
        public TimelineViewModel timelineViewModel { get; set; }

        public MainPage()
        {
            InitializeComponent();
            appSettings = new AppSettings();
            applicationBarViewModel = new ApplicationBarViewModel(this);
            trendMapViewModel = new TrendMapViewModel(this);
            timelineViewModel = new TimelineViewModel(this);
            searchBarViewModel = new SearchBarViewModel(trendMapViewModel, SearchBar);

            
            this.currentTimestamp = NormalizeTimestamp(DateTime.Now);

            Debug.WriteLine("\nZoom Level: {0}", TrendMap.ZoomLevel);
            // this.Content = UnitTestSystem.CreateTestPage();
        }

        private DateTime NormalizeTimestamp(DateTime time)
        {
            return new DateTime(time.Year, time.Month, time.Day, time.Hour, 0, 0);
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