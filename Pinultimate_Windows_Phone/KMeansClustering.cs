using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 

namespace Pinultimate_Windows_Phone
{
    class ClusteringProcessor
    {
        // TODO
        public static List<ClusteringProcessor> InitClusteringProcessors(QueryResult<GridLocationData> queryResult)
        {
            return null;
        }

        private int K { get; set; }
        private ResponseData<GridLocationData> Data { get; set; }
        private List<GridLocationData> LocationData {
            get {
                return (List<GridLocationData>)Data.LocationData;
            }
        }

        private class ClusterCenter
        {
            public static ClusterCenter FindCenter(List<GridLocationData> locations)
            {
                int count = 0;
                double latitude = 0;
                double longitude = 0;
                foreach (GridLocationData location in locations)
                {
                    count += location.Count;
                    latitude += location.Latitude * location.Count;
                    longitude += location.Longitude * location.Count;
                }
                return new ClusterCenter(latitude/count, longitude/count);
            }

            public double Latitude { get; set; }
            public double Longitude { get; set; }

            public ClusterCenter(double lat, double lon)
            {
                Latitude = lat;
                Longitude = lon;
            }

            public double Distance(GridLocationData datum)
            {
                double dist = Math.Sqrt(Math.Pow(Latitude - datum.Latitude, 2) + Math.Pow(Longitude - datum.Longitude, 2));
                return dist;
            }

            public bool Equals(ClusterCenter other)
            {
                return Latitude == other.Latitude && Longitude == other.Longitude;
            }
        }

        private ClusterCenter FindCluster(GridLocationData location, List<ClusterCenter> centers)
        {
            double minDistance = Double.PositiveInfinity;
            ClusterCenter result = null;
            foreach(ClusterCenter center in centers)
            {
                double distance = center.Distance(location);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    result = center;
                }
            }
            return result;
        }

        private void UpdateClusterCenters(Dictionary<ClusterCenter, List<GridLocationData>> clusters)
        {
            List<ClusterCenter> originalCenters = clusters.Keys.ToList<ClusterCenter>();
            foreach(ClusterCenter center in originalCenters)
            {
                List<GridLocationData> locations = clusters[center];
                ClusterCenter newCenter = ClusterCenter.FindCenter(locations);
                clusters.Remove(center);
                clusters.Add(newCenter, locations);
            }
        }

        public List<Cluster> Cluster()
        {
            List<ClusterCenter> centers = InitClusters();
            Dictionary<ClusterCenter, List<GridLocationData>> clusters = new Dictionary<ClusterCenter, List<GridLocationData>>();
            foreach (ClusterCenter center in centers)
            {
                clusters.Add(center, new List<GridLocationData>());
            }
            foreach (GridLocationData location in LocationData)
            {
                ClusterCenter center = FindCluster(location, clusters.Keys.ToList<ClusterCenter>());
                List<GridLocationData> locations = clusters[center];
                locations.Add(location);
                clusters.Add(center, locations);
            }
            UpdateClusterCenters(clusters);

            while (true)
            {
                Dictionary<ClusterCenter, List<GridLocationData>> newClusters = new Dictionary<ClusterCenter,List<GridLocationData>>();
                foreach (ClusterCenter center in clusters.Keys)
                {
                    newClusters.Add(center, new List<GridLocationData>());
                }
                int reassignments = 0;
                foreach (ClusterCenter center in clusters.Keys)
                {
                    List<GridLocationData> locations = clusters[center];
                    foreach (GridLocationData location in locations)
                    {
                        ClusterCenter newCenter = FindCluster(location, newClusters.Keys.ToList<ClusterCenter>());       
                        List<GridLocationData> newLocations = newClusters[newCenter];
                        newLocations.Add(location);
                        newClusters.Add(newCenter, newLocations);

                        if (!newCenter.Equals(center))
                        {
                            reassignments++;
                        }
                    }
                }

                if (reassignments > 0) {
                    UpdateClusterCenters(newClusters);
                    clusters = newClusters;
                } else {
                    break;
                }
            }
            
            List<Cluster> results = new List<Cluster>();
            foreach (ClusterCenter center in clusters.Keys)
            {
                Cluster cluster = new Cluster(center.Latitude, center.Longitude, clusters[center].Count);
                results.Add(cluster);
            }
            return results;
        }

        private List<ClusterCenter> InitClusters()
        {
            List<ClusterCenter> clusters = new List<ClusterCenter>();
            int n = Data.LocationData.Count();
            Random rand = new Random();
            for (int i = 0; i < K; i++)
            {
                GridLocationData checkin = Data.LocationData.ElementAt(rand.Next(0, n));
                ClusterCenter cluster = new ClusterCenter(checkin.Latitude, checkin.Longitude);
                clusters.Add(cluster);
            }
            return clusters;
        }





    }
}
