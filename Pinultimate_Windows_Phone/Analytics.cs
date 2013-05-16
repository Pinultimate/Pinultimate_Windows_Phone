using Microsoft.Phone.Info;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Pinultimate_Windows_Phone
{
    public class Analytics
    {
        private static const String ANALYTICS_URL = "http://api.pinultimate.net/analytics/";
        private static const String UUID_KEY = "uuid";

        private static readonly IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

        public static void open()
        {
            Post(ANALYTICS_URL + "open_phone/" + UUID(), "", null);
        }

        public static void tap()
        {
            Post(ANALYTICS_URL + "tap_phone/" + UUID(), "", null);
        }

        public static void close()
        {
            Post(ANALYTICS_URL + "close_phone/" + UUID(), "", null);
        }

        private static String UUID()
        {
            if (!settings.Contains(UUID_KEY))
            {
                settings.Add(UUID_KEY, getDeviceId());
            }
            return (String)settings[UUID_KEY];
        }

        private static String getDeviceId()
        {
            byte[] id = (byte[])DeviceExtendedProperties.GetValue("DeviceUniqueId");
            return Convert.ToBase64String(id);
        }

        private static async void Post(string address, string parameters, Action<string> onResponseGot)
        {
            Uri uri = new Uri(address);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";

            // Write the request Asynchronously 
            using (var stream = await Task.Factory.FromAsync<Stream>(request.BeginGetRequestStream,
                                                          request.EndGetRequestStream, null))
            {
                await stream.WriteAsync(new byte[] { }, 0, 0);
            }
        }
    }
}
