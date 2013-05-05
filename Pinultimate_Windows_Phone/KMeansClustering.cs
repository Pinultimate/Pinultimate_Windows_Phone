using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pinultimate_Windows_Phone.Data;

namespace Pinultimate_Windows_Phone
{
    public class ClusteringProcessor
    {

        #region "Public API for ClusteringProcessor"

        private const int TESTING_K = 1;

        private int K { get; set; }
        private ResponseData<GridLocationData> Data { get; set; }
        private List<GridLocationData> LocationData {
            get {
                return Data.LocationData.ToList<GridLocationData>();
            }
        }

        private ClusteringProcessor(ResponseData<GridLocationData> locationData)
        {
            Data = locationData;
            K = TESTING_K;
        }
    
        public static List<ClusteringProcessor> GetClusteringProcessors(QueryResult<GridLocationData> queryResult)
        {
            List<ClusteringProcessor> toReturn = new List<ClusteringProcessor>();
            foreach (ResponseData<GridLocationData> data in queryResult.ResponseData)
            {
                ClusteringProcessor processor = new ClusteringProcessor(data);
                toReturn.Add(processor);
            }
            return toReturn;
        }

        
        public List<Cluster> Clusters()
        {
            List<ClusterCenter> centers = InitClusters();
            return ClusterLocationFromCenters(centers, LocationData);
        }

        #endregion

        #region "Internal classes, methods to generate clusters"

        private class ClusterCenter : Object
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

            public override bool Equals(Object obj)
            {
                if (obj == null || obj.GetType() != GetType()) return false;
                ClusterCenter other = (ClusterCenter)obj;
                return Latitude == other.Latitude && Longitude == other.Longitude;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public double Radius(List<GridLocationData> data)
            {
                double result = 0;
                foreach (GridLocationData datum in data)
                {
                    double radius = this.Distance(datum);
                    if (radius > result)
                    {
                        result = radius;
                    }
                }
                return result;
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
                clusters[newCenter] = locations;
            }
        }

        private List<Cluster> ClusterLocationFromCenters(List<ClusterCenter> centers, List<GridLocationData> locationData)
        {
            Dictionary<ClusterCenter, List<GridLocationData>> clusters = new Dictionary<ClusterCenter, List<GridLocationData>>();
            foreach (ClusterCenter center in centers)
            {
                clusters[center] = new List<GridLocationData>();
            }
            foreach (GridLocationData location in locationData)
            {
                ClusterCenter center = FindCluster(location, clusters.Keys.ToList<ClusterCenter>());
                List<GridLocationData> locations = clusters[center];
                locations.Add(location);
                clusters[center] = locations;
            }
            UpdateClusterCenters(clusters);

            while (true)
            {
                Dictionary<ClusterCenter, List<GridLocationData>> newClusters = new Dictionary<ClusterCenter, List<GridLocationData>>();
                foreach (ClusterCenter center in clusters.Keys)
                {
                    newClusters[center] = new List<GridLocationData>();
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
                        newClusters[newCenter] = newLocations;

                        if (!newCenter.Equals(center))
                        {
                            reassignments++;
                        }
                    }
                }

                if (reassignments > 0)
                {
                    UpdateClusterCenters(newClusters);
                    clusters = newClusters;
                }
                else
                {
                    break;
                }
            }

            List<Cluster> results = new List<Cluster>();
            foreach (ClusterCenter center in clusters.Keys)
            {
                double radius = center.Radius(clusters[center]);
                Cluster cluster = new Cluster(center.Latitude, center.Longitude, clusters[center].Count, radius);
                results.Add(cluster);
            }
            return results;
        }

        private double calculateCost(List<ClusterCenter> centers, Dictionary<GridLocationData, double> distances,  List<GridLocationData> locationData) 
        {
            double sum = 0;
            foreach (GridLocationData checkin in locationData)
            {
                ClusterCenter cluster = FindCluster(checkin, centers);
                double dist = Math.Pow(cluster.Distance(checkin), 2);
                sum += dist;
                distances[checkin] = dist;
            }
            return sum;
        } 

        private List<ClusterCenter> InitClusters()
        {
            List<ClusterCenter> centers = KMeansLine();
            return ReCluster(centers);
        }

        private List<ClusterCenter> ReCluster(List<ClusterCenter> oldCenters)
        {
            List<GridLocationData> oldCentersLocation = new List<GridLocationData>();
            foreach (ClusterCenter oldCenter in oldCenters)
            {
                GridLocationData data = new GridLocationData();
                data.Latitude = oldCenter.Latitude;
                data.Longitude = oldCenter.Longitude;
                data.Count = 1;
                oldCentersLocation.Add(data);
            }

            List<ClusterCenter> centers = KMeansPlus(oldCentersLocation, oldCenters);

            List<Cluster> clusters = ClusterLocationFromCenters(centers, oldCentersLocation);
            List<ClusterCenter> result = new List<ClusterCenter>();
            foreach (Cluster c in clusters)
            {
                ClusterCenter cen = new ClusterCenter(c.Latitude, c.Longitude);
                result.Add(cen);
            }

            return result;
        }

        // Initialize using K-Means||
        private List<ClusterCenter> KMeansLine()
        {
            int NUM_ITERATIONS = 5; //Can be 5 - 10 
            int l = K / 2;

            List<ClusterCenter> centers = new List<ClusterCenter>();
            Dictionary<GridLocationData, double> distances = new Dictionary<GridLocationData, double>();
            Random rand = new Random();
            GridLocationData initCenter = Data.LocationData.ElementAt(new Random().Next(Data.LocationData.Count()));
            ClusterCenter initCluster = new ClusterCenter(initCenter.Latitude, initCenter.Longitude);
            centers.Add(initCluster);
            double sum = calculateCost(centers, distances, Data.LocationData.ToList());

            for (int i = 0; i < NUM_ITERATIONS; i++)
            {
                foreach (GridLocationData checkin in Data.LocationData)
                {
                    double prob = rand.NextDouble() * sum * l;
                    if (centers.Contains(new ClusterCenter(checkin.Latitude, checkin.Longitude))) continue;
                    if (prob - distances[checkin] > 0) continue;
                    ClusterCenter newCenter = new ClusterCenter(checkin.Latitude, checkin.Longitude);
                    centers.Add(newCenter);
                }
                sum = calculateCost(centers, distances, Data.LocationData.ToList());
            }
            return centers;
        }

        // Reinitialize using K-Means++
        private List<ClusterCenter> KMeansPlus(List<GridLocationData> oldCentersLocation, List<ClusterCenter> oldCenters)
        {
            List<ClusterCenter> centers = new List<ClusterCenter>();
            GridLocationDataEqCoordinates comparator = new GridLocationDataEqCoordinates();
            Dictionary<GridLocationData, int> weights = new Dictionary<GridLocationData, int>(comparator);
            Dictionary<GridLocationData, double> distances = new Dictionary<GridLocationData, double>();

            double totalWeight = Data.LocationData.Count();

            foreach (GridLocationData checkin in Data.LocationData)
            {
                ClusterCenter nearestCenter = FindCluster(checkin, oldCenters);
                GridLocationData nearestLocation = new GridLocationData();
                nearestLocation.Latitude = nearestCenter.Latitude;
                nearestLocation.Longitude = nearestCenter.Longitude;
                nearestLocation.Count = 1;
                if (weights.ContainsKey(nearestLocation))
                {
                    weights[nearestLocation] = weights[nearestLocation] + 1;
                }
                else
                {
                    weights.Add(nearestLocation, 1);
                }
            }

            ClusterCenter initCluster = oldCenters.ElementAt(new Random().Next(oldCenters.Count));
            centers.Add(initCluster);
            double sum = calculateCost(centers, distances, oldCentersLocation);
            bool flag = true;
            while (true)
            {
                foreach (GridLocationData oldCenter in oldCentersLocation)
                {
                    double prob = new Random().NextDouble() * sum * weights[oldCenter] / totalWeight;
                    if (centers.Contains(new ClusterCenter(oldCenter.Latitude, oldCenter.Longitude))) continue;
                    if (prob - distances[oldCenter] > 0) continue;
                    ClusterCenter newCenter = new ClusterCenter(oldCenter.Latitude, oldCenter.Longitude);
                    centers.Add(newCenter);
                    flag = false;
                    if (centers.Count() == K) {
                        flag = true;
                        break;
                    }
                }
                sum = calculateCost(centers, distances, oldCentersLocation);
                // flag is true if 1) centers.Count() == K or centers.Count() did not change after an iteration
                if (flag) break;
            }
            return centers;
        }

        #endregion
    }
}
