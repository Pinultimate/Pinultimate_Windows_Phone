namespace Pinultimate_Windows_Phone.Data
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Device.Location;

    /// <summary>
    /// List of stores
    /// </summary>
    public class ClusterList : ObservableCollectionEx<Cluster>
    {
        public event NotifyCollectionChangedEventHandler ClustersChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClusterList"/> class
        /// </summary>
        public ClusterList(NotifyCollectionChangedEventHandler clustersHandler)
        {
            this.CollectionChanged += ClusterList_CollectionChanged;
            this.ClustersChanged += clustersHandler;
        }

        public ClusterList()
        {
            this.CollectionChanged += ClusterList_CollectionChanged;
        }

        public ClusterList(List<Cluster> clusters)
        {
            this.CollectionChanged += ClusterList_CollectionChanged;
            AddResults(clusters);
        }

        private void ClusterList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ClustersChanged.GetInvocationList().Length > 0)
            {
                ClustersChanged(sender, e);
            }
        }

        public void AddResults(List<Cluster> results)
        {
            this.Clear();
            using (ObservableCollectionEx<Cluster> temp = this.DelayNotifications())
            {
                foreach (Cluster result in results)
                {
                    temp.Add(result);
                }
            }
        }
    }
}
