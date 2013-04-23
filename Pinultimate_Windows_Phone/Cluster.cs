using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinultimate_Windows_Phone
{
    class Cluster
    {
        public Cluster(double latitude, double longitude, int count, double radius)
        {
            Latitude = latitude;
            Longitude = longitude;
            Count = count;
            Radius = radius;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Count { get; set; }
        public double Radius { get; set; }

        public void draw()
        {
            return;
        }
    }
}
