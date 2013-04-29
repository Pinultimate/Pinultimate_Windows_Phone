using Microsoft.Phone.Maps.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Pinultimate_Windows_Phone.Data
{
    public class TrendMapViewModel
    {
        public Map trendMap { get; set; }
        public ClusterList clusterList { get; set; }

        public TrendMapViewModel(Map TrendMap)
        {
            clusterList = new ClusterList();
            trendMap = TrendMap;
        }

    }
}
