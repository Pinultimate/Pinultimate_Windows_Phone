using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Pinultimate_Windows_Phone
{
    public static class NavigationUtils
    {
        private static object _navigationData = null;

        public static void Navigate(this NavigationService service, string page, object data)
        {
            _navigationData = data;
            service.Navigate(new Uri(page, UriKind.Relative));
        }

        public static object GetLastNavigationData(this NavigationService service)
        {
            object data = _navigationData;
            _navigationData = null;
            return data;
        }
    }
}
