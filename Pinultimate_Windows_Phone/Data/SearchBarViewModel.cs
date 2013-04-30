using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Pinultimate_Windows_Phone.Data
{
    public class SearchBarViewModel
    {
        private String defaultSearchText { get { return "Find Locations..."; } }
        private Boolean hasSearchInput { get; set; }
        public TrendMapViewModel trendMapViewModel { get; set; }
        public TextBox searchBar { get; set; }

        public SearchBarViewModel(TrendMapViewModel TrendMapViewModel, TextBox SearchBar)
        {
            trendMapViewModel = TrendMapViewModel;
            searchBar = SearchBar;
            searchBar.FontSize = 22;
            FillDefaultSearchBarText();
            searchBar.SelectionChanged += SearchBar_SelectionChanged;
            searchBar.GotFocus += SearchBar_GotFocus;
            searchBar.LostFocus += SearchBar_LostFocus;
            searchBar.KeyUp += SearchBar_KeyUp;
            searchBar.TextChanged += SearchBar_TextChanged;
            searchBar.Visibility = Visibility.Collapsed;
            hasSearchInput = false;
        }

        private void SearchBar_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (!hasSearchInput)
            {
                if (searchBar.SelectionStart != 0 || searchBar.SelectionLength != 0)
                {
                    searchBar.Select(0, 0);
                }
            }
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!hasSearchInput && !searchBar.Text.Equals(defaultSearchText))
            {
                hasSearchInput = true;
                String text = searchBar.Text.Replace(defaultSearchText, "");
                ReplaceDefaultSearchBarText(text);
                searchBar.Select(searchBar.Text.Length, 0);
            }
            else if (searchBar.Text == String.Empty)
            {
                hasSearchInput = false;
                FillDefaultSearchBarText();
            }
        }

        private void SearchBar_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (hasSearchInput)
                {
                    // Perform search with Search_Bar text
                    trendMapViewModel.trendMap.Focus();
                }
            }
        }

        private void SearchBar_LostFocus(object sender, RoutedEventArgs e)
        {
            searchBar.Visibility = Visibility.Collapsed;
        }

        private void SearchBar_GotFocus(object sender, RoutedEventArgs e)
        {
            if (hasSearchInput)
            {
                searchBar.SelectAll();
            }
        }

        private void FillDefaultSearchBarText()
        {
            searchBar.Text = defaultSearchText;
            searchBar.FontStyle = FontStyles.Italic;
            SolidColorBrush greyBrush = new SolidColorBrush();
            greyBrush.Color = Colors.LightGray;
            searchBar.Foreground = greyBrush;
        }

        private void ReplaceDefaultSearchBarText(String text)
        {
            searchBar.Text = text;
            searchBar.FontStyle = FontStyles.Normal;
            SolidColorBrush blackBrush = new SolidColorBrush();
            blackBrush.Color = Colors.Black;
            searchBar.Foreground = blackBrush;
        }
    }
}
