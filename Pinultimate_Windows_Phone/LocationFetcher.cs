using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinultimate_Windows_Phone
{
    class LocationFetcher
    {

        public LocationFetcher()
        {
            ServerURL = "www.pinultimate.net";
        }

        public string ServerURL { private get; set; }

        public Cluster[] FetchClusters(double latitude, double longitude, double size, double resolution)
        {
            string query = CreateClusterQuery(latitude, longitude, size, resolution);
            WebRequest request = WebRequest.Create(query);
            Stream response = request.G
        }

        private string CreateClusterQuery(double latitude, double longitude, double size, double resolution)
        {
            StringBuilder query = new StringBuilder(ServerURL);
            query.Append("search/");
            return query.ToString();
        }

    }
}
