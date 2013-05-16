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
using Pinultimate_Windows_Phone.Data;
using System.ComponentModel;

namespace Pinultimate_Windows_Phone
{
    public class LocationFetcher
    {
        private const double GRANULARITY = 15;

        public LocationFetcher() { }

        public LocationFetcher(JSONLoadingStartedHandler startingCallback, JSONLoadingCompletionHandler completionCallback, JSONLoadingErrorHandler errorCallback)
        {
            instantiateWebClient();
            startingHandler = startingCallback;
            completionHandler = completionCallback;
            errorHandler = errorCallback;
        }

        // Callback functions when HTTP request starts, ends with success, or ends with an error
        private JSONLoadingStartedHandler startingHandler { get; set; }
        private JSONLoadingCompletionHandler completionHandler { get; set; }
        private JSONLoadingErrorHandler errorHandler { get; set; }
        private WebClient webClient { get; set; }
        private WebClient oldWebClient { get; set; }

        // The callback function called when the JSON has successfully been loaded and deserialized
        public delegate void JSONLoadingStartedHandler();
        public delegate void JSONLoadingCompletionHandler(QueryResult<GridLocationData> result);
        public delegate void JSONLoadingErrorHandler();

        public void FetchClusters(double latitude, double longitude, double latrange, double lonrange, DateTime min, DateTime max)
        {
            Debug.WriteLine("Initiating HTTP Request to fetch clusters");
            instantiateWebClient();
            string query = QueryURL.CreateGridQuery(latitude, longitude, latrange, lonrange, calculateResolution(latrange, lonrange));
            string subquery_timeframe = QueryURL.CreateTimeRangeQuery(min, max);
            query += subquery_timeframe;
            Debug.WriteLine("query: " + query);
            JSONResponseForURL(query);
        }

        private double calculateResolution(double width, double height)
        {
            double min = width < height ? width : height;
            return min / GRANULARITY;
        }

        private void instantiateWebClient()
        {
            webClient = new WebClient();
            webClient.DownloadProgressChanged += jsonWebClient_DownloadProgressChanged;
            webClient.DownloadStringCompleted += jsonWebClient_DownloadStringCompleted;
        }

        private void JSONResponseForURL(string url)
        {
            try
            {
                if (this.startingHandler != null)
                {
                    this.startingHandler();
                }
                webClient.DownloadStringAsync(new Uri(url));
                Debug.WriteLine("Request sent to: {0}", url);
            }
            catch (System.Net.WebException exception)
            {
                Debug.WriteLine("Exception: {0}", exception.Message);
            }
        }

        /* Automatically Generated callback for download completion */
        private void jsonWebClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                Debug.WriteLine("Download cancelled.");
                return;
            }

            if (e.Error != null)
            {
                Debug.WriteLine("Download error: {0}", e.Error.ToString());
                if (this.errorHandler != null)
                {
                    this.errorHandler();
                }
                return;
            }

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

        public void cancelWebRequest()
        {
            Debug.WriteLine("canceling web request");
            webClient.CancelAsync();
            oldWebClient = webClient;
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
    public class GridLocationData : IEquatable<GridLocationData>
    {
        [DataMember(Name = "latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "longitude")]
        public double Longitude { get; set; }

        [DataMember(Name = "count")]
        public int Count { get; set; }

        [DataMember(Name = "flickr")]
        public int Flickr { get; set; }

        [DataMember(Name = "twitter")]
        public int Twitter { get; set; }

        [DataMember(Name = "instagram")]
        public int Instagram { get; set; }

        public bool Equals(GridLocationData other)
        {
            return Latitude == other.Latitude && Longitude == other.Longitude && Count == other.Count;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class GridLocationDataEqCoordinates : EqualityComparer<GridLocationData>
    {
        public override int GetHashCode(GridLocationData data)
        {
            int hCode = (int)data.Latitude ^ (int)data.Longitude;
            return hCode.GetHashCode();
        }

        public override bool Equals(GridLocationData d1, GridLocationData d2)
        {
            return EqualityComparer<GridLocationData>.Default.Equals(d1, d2);
        }
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
            ServerURL = "http://api.pinultimate.net/";
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
