using System;
using System.Windows;
using System.Windows.Controls;

namespace Pinultimate_Windows_Phone.Data
{
    class SearchBarViewModel
    {
        public TrendMapViewModel trendMapViewModel { get; set; }
        public TextBox searchBar { get; set;}

        public SearchBarViewModel(TrendMapViewModel TrendMapViewModel, TextBox SearchBar)
        {
            trendMapViewModel = TrendMapViewModel;
            searchBar = SearchBar;
            searchBar.GotFocus += SearchBar_GotFocus;
            searchBar.LostFocus += SearchBar_LostFocus;
            searchBar.KeyUp += SearchBar_KeyUp;
        }

        private void SearchBar_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                // Perform search with Search_Bar text
                // Dismiss Search_Bar
                trendMapViewModel.trendMap.Focus();
            }
        }

        private void SearchBar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (searchBar.Text == String.Empty)
            {
                // If user didn't enter anything, re-enter place holder text
                searchBar.Text = "Search...";
            }
        }

        void SearchBar_GotFocus(object sender, RoutedEventArgs e)
        {
            searchBar.Text = "";
        }
    }
}
