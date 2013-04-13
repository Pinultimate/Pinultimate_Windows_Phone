using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinultimate_Windows_Phone
{
    class Cluster
    {
        public Cluster(double latitude, double longitude, int size)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.size = size;
        }

        public double latitude { get; set; }
        public double longitude { get; set; }
        public int size { get; set; }

        public void draw()
        {
            return;
        }
    }
}
