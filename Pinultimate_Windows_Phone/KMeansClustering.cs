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

        private class ClusterCenter
        {
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
        }

        public List<Cluster> cluster()
        {
            List<ClusterCenter> initClusters = InitClusters();
            //Dictionary<ClusterCenter, List<GridLocationData>> clusters = InitClusters();
            List<Cluster> kmeansClusters = new List<Cluster>();

            //Keep this for simplicity, will consider how to deal with generic
            IEnumerable<GridLocationData> checkins;
            if (typeof(T) == typeof(GridLocationData))
            {
                checkins = (IEnumerable<GridLocationData>)(Object)locationData;
            }
            else
            {
                return kmeansClusters;
            }

            resetK(checkins);
            initializeClusters(checkins, kmeansClusters, "naive");
            



            return kmeansClusters;
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
