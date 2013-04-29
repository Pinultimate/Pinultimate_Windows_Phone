namespace Pinultimate_Windows_Phone.Data
{
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Device.Location;

    /// <summary>
    /// List of stores
    /// </summary>
    public class ClusterList : ObservableCollectionEx<Cluster>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClusterList"/> class
        /// </summary>
        public ClusterList()
        {
            this.CollectionChanged += ClusterList_CollectionChanged;
        }

        private void ClusterList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        internal void AddResults(Cluster[] results)
        {
            this.Clear();
            foreach (Cluster result in results)
            {
                this.Add(result);
            }
        }
    }
}
