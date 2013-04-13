using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Shell;
using Pinultimate_Windows_Phone.Data;
using System;
using System.ComponentModel;
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
            SoundEffects.Initialize();
            TrendMap.Loaded += TrendMap_Loaded;
            appSettings = new AppSettings();
            CheckLocationPermission();
            applicationBarViewModel = new ApplicationBarViewModel(this);
            timelineViewModel = new TimelineViewModel(this);
            trendMapViewModel = new TrendMapViewModel(this);
            searchBarViewModel = new SearchBarViewModel(trendMapViewModel, SearchBar);


            this.currentTimestamp = NormalizeTimestamp(DateTime.Now);

            Debug.WriteLine("\nZoom Level: {0}", TrendMap.ZoomLevel);
        }

        private void CheckLocationPermission()
        {
            if (!appSettings.Contains(AppSettings.TrackerSettingKeyName))
            {
                MessageBoxResult result =
                    MessageBox.Show("This app accesses your phone's location. Is that ok?",
                    "Location",
                    MessageBoxButton.OKCancel);
                appSettings.TrackingSetting = (result == MessageBoxResult.OK);
            }
        }

        private void TrendMap_Loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "4ae744ff-a0b9-4aad-b459-8fe3fcb50bd6";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "FbGGZbw8PiMV_dUQEZjzLA";
        }

        private DateTime NormalizeTimestamp(DateTime time)
        {
            return new DateTime(time.Year, time.Month, time.Day, time.Hour, 0, 0);
        }

        private void GestureListener_PinchStarted(object sender, PinchStartedGestureEventArgs e)
        {
            Debug.WriteLine("Pinch Started");
            trendMapViewModel.cancelCurrentQuery();

        }

        private void GestureListener_PinchCompleted(object sender, PinchGestureEventArgs e)
        {
            Debug.WriteLine("Pinch Completed");
            trendMapViewModel.InitiateNewQuery();
        }

        private void GestureListener_DragStarted(object sender, DragStartedGestureEventArgs e)
        {
            Debug.WriteLine("Drag Started");
            trendMapViewModel.cancelCurrentQuery();
        }

<<<<<<< HEAD
        private void GestureListener_DragCompleted(object sender, DragCompletedGestureEventArgs e)
        {
            Debug.WriteLine("Drag Ended");
            trendMapViewModel.InitiateNewQuery();
        }

        private void GestureListener_DoubleTap(object sender, Microsoft.Phone.Controls.GestureEventArgs e)
        {
            Debug.WriteLine("Double Tap");
            trendMapViewModel.cancelCurrentQuery();
            trendMapViewModel.InitiateNewQuery();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!e.IsNavigationInitiator)
            {
                Analytics.open();
            }
            else
            {
                trendMapViewModel.RelocateAndRedrawMe();
            }
=======
        private void accessAPI()
        {
            string url = "http://www.sportsgametimes.com/games/nba/1";

            Debug.WriteLine("\nURL: {0}", url);
            WebRequest requestToServer = WebRequest.Create(url);
        }

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            Debug.WriteLine("Hello!");
            showCurrentLocationOnMap();
            accessAPI();
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
>>>>>>> Learning the environment
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (!e.IsNavigationInitiator)
            {
                Analytics.close();
            }
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            if (trendMapViewModel.SearchResultsPresent())
            {
                // Cancel the back button press
                e.Cancel = true;
                trendMapViewModel.ClearSearchResultsFromMap();

                // No history, allow the back button
                // Or do whatever you need to do, like navigate the application page
                return;
            }
        }
    }
}