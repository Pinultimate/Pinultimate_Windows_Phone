using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Pinultimate_Windows_Phone
{
    class GeoTracker
    {
        private Geolocator geolocator;
        private bool tracking;


        public void StartTracking()
        {
            this.geolocator = new Geolocator();
            this.geolocator.DesiredAccuracy = PositionAccuracy.High;
            this.geolocator.MovementThreshold = 100; // The units are meters.

            this.geolocator.StatusChanged += geolocator_StatusChanged;
            this.geolocator.PositionChanged += geolocator_PositionChanged;

            this.tracking = true;
        }

        public void StopTracking()
        {
            this.geolocator.PositionChanged -= geolocator_PositionChanged;
            this.geolocator.StatusChanged -= geolocator_StatusChanged;
            this.geolocator = null;

            this.tracking = false;
        }

        public bool IsTracking()
        {
            return this.tracking;
        }

        public async Task<GeoCoordinate> GetCurrentLocation()
        {
            // Get current location
            if (this.tracking)
            {
                Debug.Assert(this.geolocator != null);
                Geoposition position = await this.geolocator.GetGeopositionAsync();
                Geocoordinate coordinate = position.Coordinate;
                return CoordinateConverter.ConvertGeocoordinate(coordinate);
            }
            return null;            
        }

        public GeoTracker()
        {
            this.tracking = false;
        }
    }
}
