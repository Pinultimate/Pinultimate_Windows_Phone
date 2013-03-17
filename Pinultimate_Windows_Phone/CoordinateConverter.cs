using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Pinultimate_Windows_Phone
{
    public static class CoordinateConverter
    {
        public static GeoCoordinate ConvertGeocoordinate(Geocoordinate geocoordinate)
        {
            return new GeoCoordinate
                (
                geocoordinate.Latitude, geocoordinate.Longitude, geocoordinate.Altitude ?? Double.NaN,
                geocoordinate.Accuracy, geocoordinate.AltitudeAccuracy ?? Double.NaN, geocoordinate.Speed ?? Double.NaN,
                geocoordinate.Heading ?? Double.NaN
                );

        }
    }
}
