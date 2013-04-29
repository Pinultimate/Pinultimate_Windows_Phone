using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Pinultimate_Windows_Phone.Data
{
    class SearchBarViewModel
    {
        public TextBox searchBar { get; set;}

        public SearchBarViewModel(TextBox SearchBar)
        {
            // TODO: Complete member initialization
            searchBar = SearchBar;
        }
        
    }
}
