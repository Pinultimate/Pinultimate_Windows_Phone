using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Pinultimate_Windows_Phone
{
    class KMeansClustering<T>
    {

        private IEnumerable<ResponseData<T>> data;
        private int k; 
        private Dictionary<string, IEnumerable<Cluster>> clusters;

        public KMeansClustering() 
        {
            data = null;
            k = 1;
            clusters = new Dictionary<string, IEnumerable<Cluster>>();
        }

        

        public int K { 
            get; 
            set {
                k = value;
            }
        }

        // Set data through query
        public QueryResult<T> Query 
        { 
            get; 
            set {
                data = value.ResponseData;
            }
        }


        // Set data through responseData
        public IEnumerable<ResponseData<T>> ResponseData
        {
            get;
            set
            {
                data = value;
            }
        }


        public void clusteringAllTime()
        {
            if (data == null) return;

            foreach (ResponseData<T> timeSlice in data) 
            {
                IEnumerable<Cluster> clusterAtTime = clustering(timeSlice.LocationData);
                String time = timeSlice.RawTimestamp;
                clusters.Add(time, clusterAtTime);
            }
        }


        public List<Cluster> clustering(IEnumerable<T> locationData)
        {
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

        private void resetK(IEnumerable<GridLocationData> checkins) 
        {
        }

        private void initializeClusters(IEnumerable<GridLocationData> checkins, List<Cluster> kmeansClusters, string method)
        {
            if (method.Equals("naive")  )
            {
                int n = checkins.Count();
                Random rand = new Random();
                for (int i = 0; i < k; i++)
                {
                    GridLocationData checkin = checkins.ElementAt(rand.Next(0, n));
                    kmeansClusters.Add(new Cluster(checkin.Latitude, checkin.Longitude, checkin.Count)); 
                }
            } else {
                int n = checkins.Count();
                for (int i = 0; i < k; i++)
                {
                    kmeansClusters.Add(new Cluster(1, 1, 1));
                }
            }
            
        }





    }
}
