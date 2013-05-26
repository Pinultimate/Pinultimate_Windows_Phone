using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Pinultimate_Windows_Phone
{
    public partial class SettingsPanorama : PhoneApplicationPage
    {
        public SettingsPanorama()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void Color_Selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            AppSettings appSettings = (AppSettings) NavigationUtils.GetLastNavigationData(NavigationService);
            DataContext = appSettings;
        }
    }
}