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
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Windows.Media;

namespace Pinultimate_Windows_Phone
{
    public partial class ClusterInformationPanorama : PhoneApplicationPage
    {
        private readonly LocationFetcher locationFetcher;
        private Cluster cluster { get; set; }

        private TextBlock notification;


        public ClusterInformationPanorama()
        {
            InitializeComponent();
            locationFetcher = new LocationFetcher(
                typeof(TweetsResult),
               new Pinultimate_Windows_Phone.LocationFetcher.JSONLoadingStartedHandler(fetchTweetsStartedCallback),
               new Pinultimate_Windows_Phone.LocationFetcher.JSONLoadingCompletionHandler(fetchTweetsFinishedCallback),
               new Pinultimate_Windows_Phone.LocationFetcher.JSONLoadingErrorHandler(fetchTweetsErrorCallback)
            );
            notification = MakeTextBlock("", -1);
        }

        private TextBlock MakeTextBlock(String text, int index)
        {
            TextBlock tb = new TextBlock();
            tb.Text = text;
            tb.Margin = new Thickness(10, 5, -10, 5);
            tb.TextWrapping = TextWrapping.Wrap;
            
            if (index == -1)
            {
                tb.FontSize = 15;
                tb.FontStyle = FontStyles.Italic;
            }
            else
            {
                tb.FontSize = 25;
                if (index % 2 == 0)
                {
                    tb.Foreground = (SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"];
                }
            }
            return tb;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            cluster = (Cluster) NavigationUtils.GetLastNavigationData(NavigationService);
            DataContext = cluster;
            Latitude_val.Text = FormatLatitude(cluster.Latitude);
            Longitude_val.Text = FormatLongitude(cluster.Longitude);
            Timestamp_val.Text = GetTimeDescription(cluster.Timestamp) + " " + GetDateDescription(cluster.Timestamp);
            //Radius_val.Text = cluster.Radius.ToString();
            //Count_val.Text = cluster.Count.ToString();
            Twitter_val.Text = FormatPercentage(cluster.Twitter / cluster.Count);
            Flickr_val.Text = FormatPercentage(cluster.Flickr / cluster.Count);
            Instagram_val.Text = FormatPercentage(cluster.Instagram / cluster.Count);


            // load tweets
            locationFetcher.FetchTweets(
                cluster.Latitude, cluster.Longitude,
                1,
                cluster.Timestamp, cluster.Timestamp
            );
        }

        private string FormatPercentage(double perc)
        {
            return perc.ToString("P");
        }

        private string FormatLatitude(double latitude)
        {
            string result = "";
            if (latitude > 0)
            {
                result = " N";
            }
            else if (latitude < 0)
            {
                result = " S";
            }
            double niceLatitude = Math.Truncate(Math.Abs(latitude) * 100) / 100;
            return "" + niceLatitude + result;
        }

        private string FormatLongitude(double longitude)
        {
            string result = "";
            if (longitude > 0)
            {
                result = " E";
            }
            else if (longitude < 0)
            {
                result = " W";
            }
            double niceLongitude = Math.Truncate(Math.Abs(longitude) * 100) / 100;
            return "" + niceLongitude + result;
        }

        private String GetDateDescription(DateTime dt)
        {
            DateTime current = DateTime.Now;
            if (current.Date == dt.Date)
            {
                return "Today";
            }
            else if (current.Subtract(TimeSpan.FromDays(1)).Date == dt.Date)
            {
                return "Yesterday";
            }
            else
            {
                return dt.ToString("ddd");
            }
        }

        private String GetTimeDescription(DateTime dt)
        {
            return dt.ToString("h:mm");
        }

        public void fetchTweetsStartedCallback()
        {
            Console.WriteLine("Count Count Count: ");
            notification.Text = "Pulling Tweets at this location...";
            Tweets.Children.Add(notification);
        }

        public void fetchTweetsFinishedCallback(Object resultObject)
        {
            TweetsResult result = (TweetsResult)resultObject;
            if (result.Tweets.Count() == 0)
            {
                notification.Text = "Sorry, we can't find anything from Twitter...";
            }
            else
            {
                Tweets.Children.Remove(notification);
                int index = 0;
                foreach (String tweet in result.Tweets)
                {
                    TextBlock tb = MakeTextBlock(tweet, index++);
                    Tweets.Children.Add(tb);
                }
                TweetsHeader.Header += "(" + result.Tweets.Count() + ")";
            }
        }

        private void fetchTweetsErrorCallback()
        {
            notification.Text = "We weren't able to retrieve any tweets from the server. Check to see if you have Internet access.";
        }


        // model for tweets query
        [DataContract]
        public class TweetsResult
        {
            [DataMember(Name = "response")]
            public IEnumerable<String> Tweets { get; set; }
        }
    }
}