using FHXTools.FHX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FHXTools.Views
{
    /// <summary>
    /// Logique d'interaction pour SearchWindow.xaml
    /// </summary>
    public partial class SearchWindow : Window
    {
        FHXObject root;

        public SearchWindow()
        {
            InitializeComponent();
        }

        public SearchWindow(FHXObject root)
        {
            InitializeComponent();
            this.root = root;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string query = tbSearch.Text;

            List<FHXSearchResult> results = root.Search(query);
            this.gridSearch.ItemsSource = results;
            this.gridSearch.Columns[0].Visibility = Visibility.Hidden; //Hides the Parent field
            this.gridSearch.Columns[1].Visibility = Visibility.Hidden; //Hides the Parent field
        }
    }
}
