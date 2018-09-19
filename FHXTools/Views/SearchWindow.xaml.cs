using FHXTools.FHX;
using Microsoft.Win32;
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
        List<FHXSearchResult> results = null;

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

            results = root.Search(query);
            this.gridSearch.ItemsSource = results;
            this.gridSearch.Columns[0].Visibility = Visibility.Hidden; //Hides the Parent field
            this.gridSearch.Columns[1].Visibility = Visibility.Hidden; //Hides the Parent field
        }

        private void ExportExcel(object sender, RoutedEventArgs e)
        {
            if (results == null) return;
            string sMessageBoxText = string.Format("Exporter la recherche ?");
            string sCaption = "Export";

            MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
            MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

            MessageBoxResult rsltMessageBox = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

            switch (rsltMessageBox)
            {
                case MessageBoxResult.Yes:
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Excel file (*.xlsx)|*.xlsx";
                    if (saveFileDialog.ShowDialog() == true)
                        FHXExcelExporter.ExportRecherche(results, saveFileDialog.FileName);
                    break;
            }
        }
    }
}
