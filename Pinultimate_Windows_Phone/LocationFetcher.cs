using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Pinultimate_Windows_Phone.Data.Maps;

namespace Pinultimate_Windows_Phone
{
    class LocationFetcher
    {
        public LocationFetcher() { }

        // This will be used as a callback function when the JSON data is fully downloaded
        public JSONLoadingCompletionHandler completionHandler { get; set; }

        // The callback function called when the JSON has successfully been loaded and deserialized
        public delegate void JSONLoadingCompletionHandler(QueryResult<GridLocationData> result);

        public Cluster[] FetchClusters(double latitude, double longitude, double latrange, double lonrange, double resolution)
        {
            //string query = CreateClusterQuery(latitude, longitude, latrange, lonrange, resolution);
            //WebRequest request = WebRequest.Create(query);
            return new Cluster[2]; // Just so it will compile, not an actual line of code
        }

        

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
            QueryResult<GridLocationData> queryResult = ResponseParser.Parse<GridLocationData>(result);
            Debug.WriteLine("JSON Result: {0}", result);
            // Call user supplied callback function with result
            if (this.completionHandler != null)
            {
                this.completionHandler(queryResult);
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

    [DataContract]
    public class QueryResult<T>
    {
        [DataMember(Name = "request")]
        public RequestData RequestData { get; set; }

        [DataMember(Name = "response")]
        public IEnumerable<ResponseData<T>> ResponseData { get; set; }
    }

    // currently only works when requesting grid data
    [DataContract]
    public class RequestData
    {

    }

    [DataContract]
    public class ResponseData<T>
    {
        private string _RawTimestamp;

        [DataMember(Name = "timestamp")]
        public string RawTimestamp
        {
            get { return _RawTimestamp; }
            set 
            {
                _RawTimestamp = value;
                Timestamp = DateTime.Parse(_RawTimestamp);
            }
        }

        public DateTime Timestamp { get; private set; }

        [DataMember(Name = "locations")]
        public IEnumerable<T> LocationData { get; set; }
    }

    [DataContract]
    public class GridLocationData
    {
        [DataMember(Name = "latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "longitude")]
        public double Longitude { get; set; }

        [DataMember(Name = "count")]
        public int Count { get; set; }
    }

    [DataContract]
    public class RawLocationData
    {
        [DataMember(Name = "latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "longitude")]
        public double Longitutde { get; set; }
    }

    static class ResponseParser
    {
        public static QueryResult<T> Parse<T>(string response)
        {
            var result = Activator.CreateInstance<QueryResult<T>>();
            using (var memStream = new MemoryStream(Encoding.Unicode.GetBytes(response)))
            {
                var serializer = new DataContractJsonSerializer(result.GetType());
                result = (QueryResult<T>) serializer.ReadObject(memStream);
                return result;
            }
        }
    }

    /* Query URL Generator */
    static class QueryURL
    {
        static QueryURL()
        {
            ServerURL = "http://www.pinultimate.net/";
            AppURL = "heatmap/";
        }

        public static string ServerURL { get; set; }
        public static string AppURL { get; set; }

        public static string CreateGridQuery(double latitude, double longitude, double latrange, double lonrange, double resolution)
        {
            StringBuilder query = new StringBuilder(ServerURL + AppURL);
            query.Append("resolution/" + resolution + "/");
            query.Append("search/center/" + latitude + "/" + longitude + "/region/" + latrange + "/" + lonrange + "/");
            return query.ToString();
        }

        public static string CreateRawQuery(double latitude, double longitude, double latrange, double lonrange)
        {
            StringBuilder query = new StringBuilder(ServerURL + AppURL);
            query.Append("raw/");
            query.Append("search/center/" + latitude + "/" + longitude + "/region/" + latrange + "/" + lonrange + "/");
            return query.ToString();
        }

        public static string CreateTimeRangeQuery(DateTime from, DateTime to)
        {
            StringBuilder query = new StringBuilder();
            query.Append("from/" + from.Year + "/" + from.Month + "/" + from.Day + "/" + from.Hour + "/");
            query.Append("to/" + to.Year + "/" + to.Month + "/" + to.Day + "/" + to.Hour + "/");
            return query.ToString();
        }
    }
}
