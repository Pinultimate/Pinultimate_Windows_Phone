using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Pinultimate_Windows_Phone
{
    class LocationFetcher
    {

        public LocationFetcher()
        {
            ServerURL = "http://www.pinultimate.net";
        }

        public string ServerURL { private get; set; }

        public Cluster[] FetchClusters(double latitude, double longitude, double size, double resolution)
        {
            string query = CreateClusterQuery(latitude, longitude, size, resolution);
            WebRequest request = WebRequest.Create(query);
            return new Cluster[2]; // Just so it will compile, not an actual line of code
        }

        private string CreateClusterQuery(double latitude, double longitude, double size, double resolution)
        {
            StringBuilder query = new StringBuilder(ServerURL);
            query.Append("search/");
            return query.ToString();
        }

        public void JSONResponseForURL(string url)
        {
            WebClient jsonWebClient = new WebClient();
            jsonWebClient.DownloadProgressChanged += jsonWebClient_DownloadProgressChanged;
            jsonWebClient.DownloadStringCompleted += jsonWebClient_DownloadStringCompleted;
            try
            {
                jsonWebClient.DownloadStringAsync(new Uri(url));
                Debug.WriteLine("Request sent to: {0}", url);
            }
            catch (System.Net.WebException exception)
            {
                Debug.WriteLine("Exception: {0}", exception.Message);
            }
        }

        /* Automatically Generated callback for download completion */
        void jsonWebClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            string result = e.Result;
            Debug.WriteLine("JSON Result: {0}", result);
        }

        /* Automatically Generated callback for download progress */
        void jsonWebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            long bytesRecieved = e.BytesReceived;
            long totalBytesToRecieve = e.TotalBytesToReceive;
            int progress = e.ProgressPercentage;

            Debug.WriteLine("{0}/{1} bytes Downloaded, {2}%", bytesRecieved, totalBytesToRecieve, progress);
        }

    }
}
