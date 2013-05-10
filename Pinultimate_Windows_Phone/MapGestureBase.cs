using Microsoft.Phone.Maps.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Pinultimate_Windows_Phone
{
    /// <summary>
    /// A base class for map gestures, which allows them to suppress the built-in map gestures.
    /// </summary>
    public class MapGestureBase
    {
        /// <summary>
        /// Gets or sets whether to suppress the existing gestures/
        /// </summary>
        public bool SuppressMapGestures { get; set; }

        protected Map Map { get; private set; }

        public MapGestureBase(Map map)
        {
            Map = map;
            map.Loaded += (s, e) => CrawlTree(Map);
        }

        private void CrawlTree(FrameworkElement el)
        {
            el.ManipulationDelta += MapElement_ManipulationDelta;
            el.ManipulationStarted += el_ManipulationStarted;
            //el.ManipulationCompleted += el_ManipulationCompleted;
            for (int c = 0; c < VisualTreeHelper.GetChildrenCount(el); c++)
            {
                CrawlTree(VisualTreeHelper.GetChild(el, c) as FrameworkElement);
            }
        }

        void el_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            //Debug.WriteLine("Manipulation Started");
        }

        private void el_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            //Debug.WriteLine("Manipulation Ended");
        }

        private void MapElement_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            //Debug.WriteLine("Manipulation Delta");
            if (SuppressMapGestures)
                e.Handled = true;
            else
                e.Handled = false;
        }
    }
}
