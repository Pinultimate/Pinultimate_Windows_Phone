using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Pinultimate_Windows_Phone.Data;

namespace Pinultimate_Windows_Phone
{
    public partial class ClusterInformationPanorama : PhoneApplicationPage
    {

        private Cluster cluster { get; set; }

        public ClusterInformationPanorama()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            cluster = (Cluster) NavigationUtils.GetLastNavigationData(NavigationService);
            DataContext = cluster;
        }
    }
}