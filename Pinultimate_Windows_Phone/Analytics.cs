using Microsoft.Phone.Info;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private const String ANALYTICS_URL = "http://api.pinultimate.net/analytics/";
        private const String UUID_KEY = "uuid";

        private static readonly IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

        public static void open()
        {
            Post(ANALYTICS_URL + "open_phone/" + UUID() + "/", "", null);
        }

        public static void tap()
        {
            Post(ANALYTICS_URL + "tap_phone/" + UUID() + "/", "", null);
        }

        public static void close()
        {
            Post(ANALYTICS_URL + "close_phone/" + UUID() + "/", "", null);
        }

        private static String UUID()
        {
            if (!settings.Contains(UUID_KEY))
            {
                settings.Add(UUID_KEY, getDeviceId());
                settings.Save();
            }
            return (String)settings[UUID_KEY];
        }

        private static String getDeviceId()
        {
            byte[] id = (byte[])DeviceExtendedProperties.GetValue("DeviceUniqueId");
            return BitConverter.ToString(id).Replace("-", string.Empty);
        }

        private static void Post(string address, string parameters, Action<string> onResponseGot)
        {
            Debug.WriteLine("URL: " + address);
            Uri uri = new Uri(address);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";

            request.BeginGetRequestStream(delegate(IAsyncResult req)
            {
                var outStream = request.EndGetRequestStream(req);

                using (StreamWriter w = new StreamWriter(outStream))
                    w.Write(parameters);

                request.BeginGetResponse(delegate(IAsyncResult result)
                {
                    try
                    {
                        HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result);

                        using (var stream = response.GetResponseStream())
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                //onResponseGot(reader.ReadToEnd());
                            }
                        }
                    }
                    catch
                    {
                        //onResponseGot(null);
                    }

                }, null);

            }, null);

            //using (var stream = await Task.Factory.FromAsync<Stream>(request.BeginGetRequestStream,
            //                                              request.EndGetRequestStream, null))
            //{
            //    await stream.WriteAsync(new byte[] { }, 0, 0);
            //}
        }
    }
}
