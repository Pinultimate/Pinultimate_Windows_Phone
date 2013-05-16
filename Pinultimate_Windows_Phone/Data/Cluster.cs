﻿namespace Pinultimate_Windows_Phone.Data
{
    using System.ComponentModel;
    using System.Device.Location;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using Microsoft.Phone.Maps.Controls;

    /// <summary>
    /// Store data
    /// </summary>
    public class Cluster : INotifyPropertyChanged
    {
        /// <summary>
        /// GeoCoordinate of the store
        /// </summary>
        private GeoCoordinate geoCoordinate;

        /// <summary>
        /// Event to be raised when a property value has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

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

        /// <summary>
        /// Gets or sets the GeoCoordinate of the store
        /// </summary>
        [TypeConverter(typeof(GeoCoordinateConverter))]
        public GeoCoordinate GeoCoordinate
        {
            get
            {
                if (this.geoCoordinate == null)
                {
                    this.geoCoordinate = new GeoCoordinate(Latitude, Longitude);
                }
                return this.geoCoordinate;
            }

            set
            {
                if (this.geoCoordinate != value)
                {
                    this.geoCoordinate = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Generic NotifyPropertyChanged
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
