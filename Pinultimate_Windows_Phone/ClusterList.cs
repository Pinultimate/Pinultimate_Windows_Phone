namespace Pinultimate_Windows_Phone.Data.Maps
{
    using System.Collections.ObjectModel;
    using System.Device.Location;

    /// <summary>
    /// List of stores
    /// </summary>
    public class ClusterList : ObservableCollection<Cluster>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClusterList"/> class
        /// </summary>
        public ClusterList()
        {
            this.LoadData();
        }

        /// <summary>
        /// Loads the current store data into the collection
        /// </summary>
        private void LoadData()
        {
            this.Add(new Cluster() { GeoCoordinate = new GeoCoordinate(47.6050338745117, -122.334243774414), Address = "823 First Avenue" });
            this.Add(new Cluster() { GeoCoordinate = new GeoCoordinate(47.6045697927475, -122.329885661602), Address = "700 5th Avenue" });
            this.Add(new Cluster() { GeoCoordinate = new GeoCoordinate(47.605712890625, -122.330268859863), Address = "800 5th Avenue" });
            this.Add(new Cluster() { GeoCoordinate = new GeoCoordinate(47.6015319824219, -122.335113525391), Address = "102 First Avenue S" });
            this.Add(new Cluster() { GeoCoordinate = new GeoCoordinate(47.6056594848633, -122.334243774414), Address = "999 Third Avenue" });
        }
    }
}
