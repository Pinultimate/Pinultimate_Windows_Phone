﻿using Microsoft.Phone.Maps.Controls;
using System.Device.Location;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Threading;

namespace Pinultimate_Windows_Phone.Data
{
    public class TrendMapViewModel
    {
        public ClusterList clusterList { get; set; }
        public List<ClusteringProcessor> processors { get; set; }
        public MainPage mainPage { get; set; }
        public Map trendMap
        {
            get
            {
                return mainPage.TrendMap;
            }
            set
            {
                mainPage.TrendMap = value;
            }
        }

        #region "Me"
        public GeoTracker geoTracker { get; set; }
        public GeoCoordinate meLocation { get; set; }
        //public Ellipse meIndicator { get; set; }
        public MapOverlay meIndicatorOverlay { get; set; }
        #endregion

        private readonly LocationFetcher locationFetcher = new LocationFetcher();

        public TrendMapViewModel(MainPage MainPage)
        {
            mainPage = MainPage;
            trendMap.Loaded += TrendMap_Loaded;
            trendMap.ZoomLevelChanged += TrendMap_ZoomLevelChanged;
            trendMap.CenterChanged += TrendMap_CenterChanged;
            geoTracker = new GeoTracker(mainPage);
            geoTracker.StartTracking();
            InitializeMeIndicator();
            clusterList = new ClusterList();
            clusterList.ClustersChanged += UpdateMapWithNewClusters;
        }

        public void fetchClustersCallback(QueryResult<GridLocationData> result)
        {
            processors = ClusteringProcessor.GetClusteringProcessors(result);
            // TODO: we only get the first one for now, we'll get the rest later
            // TODO: make it asynchronous
            List<Cluster> clusters = processors[0].Clusters();
            clusterList.AddResults(clusters);
        }

        #region "callbacks"

        private void TrendMap_Loaded(object sender, RoutedEventArgs e)
        {
            LocationRectangle boundingBox = GetBoundingBox();
            // TODO - try switching width and height order to correctly match lat and long, figure out proper resolution

            locationFetcher.FetchClusters(new Pinultimate_Windows_Phone.LocationFetcher.JSONLoadingCompletionHandler(fetchClustersCallback),
                boundingBox.Northwest.Latitude, boundingBox.Northwest.Longitude, boundingBox.WidthInDegrees, boundingBox.HeightInDegrees);
        }

        private void TrendMap_CenterChanged(object sender, MapCenterChangedEventArgs e)
        {
            LocationRectangle boundingBox = GetBoundingBox();
        }

        private void TrendMap_ZoomLevelChanged(object sender, MapZoomLevelChangedEventArgs e)
        {
            LocationRectangle boundingBox = GetBoundingBox();
        }
        #endregion

        #region "Map Center Change Functions"

        public void LocateMapToCoordinate(GeoCoordinate coordinate)
        {
            trendMap.Center = coordinate;
        }


        #endregion

        #region "Me Indicator"

        public async void RelocateAndRedrawMe()
        {
            meLocation = await geoTracker.GetCurrentLocation();
            meIndicatorOverlay.GeoCoordinate = meLocation;
        }

        public void LocateMapToMe()
        {
            LocateMapToCoordinate(meLocation);
        }

        private async void InitializeMeIndicator()
        {
            Task<GeoCoordinate> geoCoordinateTask = geoTracker.GetCurrentLocation();
            if (geoCoordinateTask != null)
            {
                MapLayer meIndicatorLayer = new MapLayer();
                meIndicatorOverlay = new MapOverlay();
                Ellipse meIndicator = new Ellipse();
                meIndicator.Fill = new SolidColorBrush(Colors.Blue);
                meIndicator.Height = 10;
                meIndicator.Width = 10;
                meIndicator.Opacity = 50;
                meIndicatorOverlay.Content = meIndicator;
                meIndicatorOverlay.PositionOrigin = new Point(0.5, 0.5);
                meIndicatorLayer.Add(meIndicatorOverlay);
                meLocation = await geoTracker.GetCurrentLocation();
                meIndicatorOverlay.GeoCoordinate = meLocation;
                trendMap.Layers.Add(meIndicatorLayer);
                LocateMapToMe();
            }
        }

        #endregion

        private LocationRectangle GetBoundingBox()
        {
            TimeSpan waitTime = new TimeSpan(0, 0, 0, 0, 1);
            GeoCoordinate Point1 = trendMap.ConvertViewportPointToGeoCoordinate(new Point(0, 0));
            //GeoCoordinate Point2 = trendMap.ConvertViewportPointToGeoCoordinate(new Point(trendMap.ActualHeight, trendMap.ActualWidth));
            GeoCoordinate Point2 = trendMap.ConvertViewportPointToGeoCoordinate(new Point(696, 480));
            while (Point1 == null || Point2 == null)
            {
                Thread.Sleep(waitTime);
            }
            return new LocationRectangle(Point1, Point2);
        }

        private void UpdateMapWithNewClusters(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (Cluster cluster in clusterList)
            {
                DrawCluster(cluster);
            }
        }

        private void DrawCluster(Cluster cluster)
        {
            trendMap.Layers.Add(TrendMapDrawingUtils.CreateMapLayerForCluster(cluster));
        }
    }
}
