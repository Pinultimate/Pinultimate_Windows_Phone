using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Pinultimate_Windows_Phone.Data
{
    public class ApplicationBarViewModel
    {
        public ApplicationBarViewModel(MainPage MainPage)
        {
            mainPage = MainPage;
            applicationBar = new ApplicationBar();
            applicationBar.Mode = ApplicationBarMode.Default;
            applicationBar.Opacity = 1.0;
            applicationBar.IsVisible = true;
            applicationBar.IsMenuEnabled = false;
            ApplicationBarIconButton reloadButton = CreateIconButton(new Uri("Images/refresh.png", UriKind.Relative), "Reload");
            reloadButton.Click += ReloadButton_Click;
            ApplicationBarIconButton meButton = CreateIconButton(new Uri("/Images/share.png", UriKind.Relative), "Me");
            meButton.Click += MeButton_Click;
            ApplicationBarIconButton settingsButton = CreateIconButton(new Uri("/Images/settings.png", UriKind.Relative), "Settings");
            settingsButton.Click += SettingsButton_Click;
            ApplicationBarIconButton searchButton = CreateIconButton(new Uri("/Images/feature.search.png", UriKind.Relative), "Search");
            searchButton.Click += SearchButton_Click;

            // IMPORTANT: the order in which the buttons are added is important.
            applicationBar.Buttons.Add(reloadButton);
            applicationBar.Buttons.Add(settingsButton);
            applicationBar.Buttons.Add(meButton);
            applicationBar.Buttons.Add(searchButton);
        }

        public MainPage mainPage { get; set; }

        public IApplicationBar applicationBar
        {
            get
            {
                return mainPage.ApplicationBar;
            }

            set
            {
                mainPage.ApplicationBar = value;
            }
        }

        private TrendMapViewModel trendMapViewModel
        {
            get
            {
                return mainPage.trendMapViewModel;
            }
            set
            {
                mainPage.trendMapViewModel = value;
            }
        }

        private SearchBarViewModel searchBarViewModel
        {
            get
            {
                return mainPage.searchBarViewModel;
            }
            set
            {
                mainPage.searchBarViewModel = value;
            }
        }

        private ApplicationBarIconButton CreateIconButton(Uri iconUri, String text)
        {
            ApplicationBarIconButton button = new ApplicationBarIconButton();
            button.IconUri = iconUri;
            button.Text = text;
            return button;
        }

        private void ReloadButton_Click(object sender, EventArgs e)
        {
            ApplicationBarIconButton reloadButton = (ApplicationBarIconButton)applicationBar.Buttons[0];
            // reloads the current view
            //trendMapViewModel.refresh();
            trendMapViewModel.RelocateAndRedrawMe();
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            //ApplicationBarIconButton settingsButton = (ApplicationBarIconButton)applicationBar.Buttons[1];
            Uri settingsUri = new Uri("/SettingsPanorama.xaml", UriKind.Relative);
            mainPage.NavigationService.Navigate(settingsUri);
        }

        private void MeButton_Click(object sender, EventArgs e)
        {
            ApplicationBarIconButton meButton = (ApplicationBarIconButton)applicationBar.Buttons[2];
            trendMapViewModel.LocateMapToMe();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            ApplicationBarIconButton meButton = (ApplicationBarIconButton)applicationBar.Buttons[3];
            if (searchBarViewModel.searchBar.Visibility == System.Windows.Visibility.Collapsed)
            {
                searchBarViewModel.searchBar.Visibility = System.Windows.Visibility.Visible;
            }
            searchBarViewModel.searchBar.Focus();
        }

        /*
        private void MeButton_Click(object sender, EventArgs e)
        {
            ApplicationBarIconButton meButton = (ApplicationBarIconButton)applicationBar.Buttons[2];
            if ((bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"] != true)
            {
                // The user has opted out of Location.
                return;
            }
            if (!trendMapViewModel.geoTracker.IsTracking())
            {
                trendMapViewModel.geoTracker.StartTracking();
                trendMapViewModel.DrawCurrentLocation();
                meButton.Text = "don't track";
                btn.IconUri = new Uri("/Images/stop.png", UriKind.Relative);
            }
            else
            {
                Debug.Assert(btn.Text == "don't track");
                geoTracker.StopTracking();
                btn.Text = "track";
                btn.IconUri = new Uri("/Images/start.png", UriKind.Relative);
            }
        }*/

    }
}
