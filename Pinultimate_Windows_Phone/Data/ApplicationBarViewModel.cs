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
            applicationBar.IsMenuEnabled = true;

            ApplicationBarMenuItem reloadMenuItem = CreateMenuItem("Reload");
            reloadMenuItem.Click += ReloadMenuItem_Click;
            ApplicationBarMenuItem settingsMenuItem = CreateMenuItem("Settings");
            settingsMenuItem.Click += SettingsMenuItem_Click;

            ApplicationBarIconButton meButton = CreateIconButton(new Uri("/Images/location.png", UriKind.Relative), "Me");
            meButton.Click += MeButton_Click;
            ApplicationBarIconButton searchButton = CreateIconButton(new Uri("/Images/search.png", UriKind.Relative), "Search");
            searchButton.Click += SearchButton_Click;
            ApplicationBarIconButton backwardButton = CreateIconButton(new Uri("/Images/play.backward.png", UriKind.Relative), "Previous");
            backwardButton.Click += BackwardButton_Click;
            ApplicationBarIconButton forwardButton = CreateIconButton(new Uri("/Images/play.forward.png", UriKind.Relative), "Next");
            forwardButton.Click += ForwardButton_Click;

            // IMPORTANT: the order in which the buttons are added is important.
            applicationBar.Buttons.Add(meButton);
            applicationBar.Buttons.Add(searchButton);
            applicationBar.Buttons.Add(backwardButton);
            applicationBar.Buttons.Add(forwardButton);

            applicationBar.MenuItems.Add(reloadMenuItem);
            applicationBar.MenuItems.Add(settingsMenuItem);

            meButton.IsEnabled = false;
            backwardButton.IsEnabled = false;
            forwardButton.IsEnabled = false;
            reloadMenuItem.IsEnabled = false;
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

        private TimelineViewModel timelineViewModel
        {
            get
            {
                return mainPage.timelineViewModel;
            }
            set
            {
                mainPage.timelineViewModel = value;
            }
        }

        private ApplicationBarIconButton CreateIconButton(Uri iconUri, String text)
        {
            ApplicationBarIconButton button = new ApplicationBarIconButton();
            button.IconUri = iconUri;
            button.Text = text;
            return button;
        }

        private ApplicationBarMenuItem CreateMenuItem(String text)
        {
            ApplicationBarMenuItem menuItem = new ApplicationBarMenuItem();
            menuItem.Text = text;
            return menuItem;
        }

        private void ReloadMenuItem_Click(object sender, EventArgs e)
        {
            ApplicationBarMenuItem reloadMenuItem = (ApplicationBarMenuItem)applicationBar.MenuItems[0];
            // reloads the current view
            //trendMapViewModel.refresh();
            trendMapViewModel.RelocateAndRedrawMe();
        }

        private void SettingsMenuItem_Click(object sender, EventArgs e)
        {
            ApplicationBarMenuItem settingsMenuItem = (ApplicationBarMenuItem)applicationBar.MenuItems[1];
            NavigationUtils.Navigate(mainPage.NavigationService, "/SettingsPanorama.xaml", mainPage.appSettings); 
        }

        private void MeButton_Click(object sender, EventArgs e)
        {
            ApplicationBarIconButton meButton = (ApplicationBarIconButton)applicationBar.Buttons[0];
            //trendMapViewModel.LocateMapToMe();
            trendMapViewModel.RelocateAndRedrawMe();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            ApplicationBarIconButton meButton = (ApplicationBarIconButton)applicationBar.Buttons[1];
            if (searchBarViewModel.searchBar.Visibility == System.Windows.Visibility.Collapsed)
            {
                searchBarViewModel.searchBar.Visibility = System.Windows.Visibility.Visible;
            }
            searchBarViewModel.searchBar.Focus();
        }

        private void ForwardButton_Click(object sender, EventArgs e)
        {
            timelineViewModel.IncreaseHour();
        }

        private void BackwardButton_Click(object sender, EventArgs e)
        {
            timelineViewModel.DecreaseHour();
        }

        #region "Button Enabler/Disabler"
        public void ConfigureTimelineButtonsOnCondition()
        {
            int value = timelineViewModel.GetCurrentValue();
            if (value == timelineViewModel.GetMaximum())
            {
                DisableNextButton();
            }
            else if (value < timelineViewModel.GetMaximum())
            {
                EnableNextButton();
            }
            if (value == timelineViewModel.GetMinimum())
            {
                DisablePrevButton();
            }
            else if (value > timelineViewModel.GetMinimum())
            {
                EnablePrevButton();
            }
        }

        public void EnablePrevButton()
        {
            ApplicationBarIconButton prevButton = (ApplicationBarIconButton)applicationBar.Buttons[2];
            prevButton.IsEnabled = true;
        }

        public void DisablePrevButton()
        {
            ApplicationBarIconButton prevButton = (ApplicationBarIconButton)applicationBar.Buttons[2];
            prevButton.IsEnabled = false;
        }

        public void EnableNextButton()
        {
            ApplicationBarIconButton nextButton = (ApplicationBarIconButton)applicationBar.Buttons[3];
            nextButton.IsEnabled = true;
        }

        public void DisableNextButton()
        {
            ApplicationBarIconButton nextButton = (ApplicationBarIconButton)applicationBar.Buttons[3];
            nextButton.IsEnabled = false;
        }

        public void EnableMeButton()
        {
            ApplicationBarIconButton meButton = (ApplicationBarIconButton)applicationBar.Buttons[0];
            meButton.IsEnabled = true;
        }

        public void DisableMeButton()
        {
            ApplicationBarIconButton meButton = (ApplicationBarIconButton)applicationBar.Buttons[0];
            meButton.IsEnabled = false;
        }

        public void EnableReloadMenu()
        {
            ApplicationBarMenuItem reloadMenuItem = (ApplicationBarMenuItem)applicationBar.MenuItems[0];
            reloadMenuItem.IsEnabled = true;
        }

        public void DisableReloadMenu()
        {
            ApplicationBarMenuItem reloadMenuItem = (ApplicationBarMenuItem)applicationBar.MenuItems[0];
            reloadMenuItem.IsEnabled = false;
        }
        #endregion
    }
}
