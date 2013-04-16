﻿using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Windows.Devices.Geolocation;

namespace Pinultimate_Windows_Phone
{
    class GeoTracker
    {
        private Geolocator geolocator;
        private bool tracking;
        private ApplicationBar applicationBar;

        private void geolocator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            string status = "";

            switch (args.Status)
            {
                case PositionStatus.Disabled:
                    // the application does not have the right capability or the location master switch is off
                    status = "location is disabled in phone settings";
                    break;
                case PositionStatus.Initializing:
                    // the geolocator started the tracking operation
                    status = "initializing";
                    break;
                case PositionStatus.NoData:
                    // the location service was not able to acquire the location
                    status = "no data";
                    break;
                case PositionStatus.Ready:
                    // the location service is generating geopositions as specified by the tracking parameters
                    status = "ready";
                    break;
                case PositionStatus.NotInitialized:
                    // the initial state of the geolocator, once the tracking operation is stopped by the user the geolocator moves back to this state

                    break;
            }

            applicationBar.Dispatcher.BeginInvoke((Action)() =>
            {
                
                StatusTextBlock.Text = status;
            });
        }

        private void geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            Dispatcher.BeginInvoke(() =>
            {
                //LatitudeTextBlock.Text = args.Position.Coordinate.Latitude.ToString("0.00");
                //LongitudeTextBlock.Text = args.Position.Coordinate.Longitude.ToString("0.00");
            });
        }

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

        public GeoTracker(ApplicationBar applicationBar)
        {
            this.applicationBar = applicationBar;
            this.tracking = false;
        }
    }
}
