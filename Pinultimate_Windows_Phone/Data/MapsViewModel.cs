using System.Collections.Specialized;
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
        /// 
        private readonly LocationFetcher locationFetcher = new LocationFetcher();

        public MapsViewModel()
        {
            this.clusterList = new ClusterList(); 
            this.clusterList.CollectionChanged += UpdateMapWithNewClusters;
        }

        private void UpdateMapWithNewClusters(object sender, NotifyCollectionChangedEventArgs e)
        {
            // TODO update Map with new clusters
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets or sets the list of stores
        /// </summary>
        public ClusterList clusterList { get; set; }

        public void FetchLocations(double latitude, double longitude, double latrange, double lonrange, double resolution)
        {
            Cluster[] results = locationFetcher.FetchClusters(latitude, longitude, latrange, lonrange, resolution);
            clusterList.AddResults(results);
            
        }


    }
}
