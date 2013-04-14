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
            ServerURL = "http://www.pinultimate.net/";
            AppURL = "heatmap/";
        }

        public string ServerURL { get; set; }
        public string AppURL { get; set; }

        // This will be used as a callback function when the JSON data is fully downloaded
        public JSONLoadingCompletionHandler completionHandler { get; set; }

        //public delegate void JSONLoadingCompletionHandler(Object[] checkIns); REAL FORM
        public delegate void JSONLoadingCompletionHandler(string json);

        public Cluster[] FetchClusters(double latitude, double longitude, double latrange, double lonrange, double resolution)
        {
            string query = CreateClusterQuery(latitude, longitude, latrange, lonrange, resolution);
            WebRequest request = WebRequest.Create(query);
            return new Cluster[2]; // Just so it will compile, not an actual line of code
        }

        /* QUERY URL GENERATORS */
        private string CreateClusterQuery(double latitude, double longitude, double latrange, double lonrange, double resolution)
        {
            StringBuilder query = new StringBuilder(ServerURL + AppURL);
            query.Append("search/");
            return query.ToString();
        }

        private string CreateGridQuery(double latitude, double longitude, double latrange, double lonrange, double resolution)
        {
            StringBuilder query = new StringBuilder(ServerURL + AppURL);
            query.Append("resolution/" + resolution + "/");
            query.Append("search/center/" + latitude + "/" + longitude + "/region/" + latrange + "/" + lonrange + "/");
            return query.ToString();
        }

        private string CreateRawQuery(double latitude, double longitude, double latrange, double lonrange)
        {
            StringBuilder query = new StringBuilder(ServerURL + AppURL);
            query.Append("raw/");
            query.Append("search/center/" + latitude + "/" + longitude + "/region/" + latrange + "/" + lonrange + "/");
            return query.ToString();
        }

        private string CreateTimeRangeQuery(DateTime from, DateTime to)
        {
            StringBuilder query = new StringBuilder();
            query.Append("from/" + from.Year + "/" + from.Month + "/" + from.Day + "/" + from.Hour + "/");
            query.Append("to/" + to.Year + "/" + to.Month + "/" + to.Day + "/" + to.Hour + "/");
            return query.ToString();
        }

        /* QUERY URL GENERATORS */

        public void JSONResponseForURL(string url, JSONLoadingCompletionHandler callback)
        {
            // Set completion handler to supplied callback
            this.completionHandler = callback;
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
            // Call user supplied callback function with result
            if (this.completionHandler != null)
            {
                this.completionHandler(result);
            }
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
