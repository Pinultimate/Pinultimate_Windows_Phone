namespace Pinultimate_Windows_Phone.Data.Maps
{
    /// <summary>
    /// Maps Main View Model used in the map sample page
    /// </summary>
    public class MapsViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapsViewModel"/> class
        /// </summary>
        public MapsViewModel()
        {
            this.ClusterList = new ClusterList();
        }

        /// <summary>
        /// Gets or sets the list of stores
        /// </summary>
        public ClusterList ClusterList { get; set; }
    }
}
