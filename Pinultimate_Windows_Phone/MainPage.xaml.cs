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
            timelineViewModel = new TimelineViewModel(this);
            trendMapViewModel = new TrendMapViewModel(this);
            searchBarViewModel = new SearchBarViewModel(trendMapViewModel, SearchBar);

            
            this.currentTimestamp = NormalizeTimestamp(DateTime.Now);

            Debug.WriteLine("\nZoom Level: {0}", TrendMap.ZoomLevel);
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
            trendMapViewModel.initiateNewQuery();
        }

        private void GestureListener_DragStarted(object sender, DragStartedGestureEventArgs e)
        {
            Debug.WriteLine("Drag Started");
            trendMapViewModel.cancelCurrentQuery();
        }

        private void GestureListener_DragCompleted(object sender, DragCompletedGestureEventArgs e)
        {
            Debug.WriteLine("Drag Ended");
            trendMapViewModel.initiateNewQuery();
        }

        private void GestureListener_DoubleTap(object sender,  Microsoft.Phone.Controls.GestureEventArgs e)
        {
            Debug.WriteLine("Double Tap");
            trendMapViewModel.cancelCurrentQuery();
            trendMapViewModel.initiateNewQuery();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("LocationConsent"))
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
            if (!e.IsNavigationInitiator)
            {
                Analytics.open();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (!e.IsNavigationInitiator)
            {
                Analytics.close();
            }
        }

    }
}