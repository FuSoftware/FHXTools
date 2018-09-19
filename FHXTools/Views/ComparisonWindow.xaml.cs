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
    /// Logique d'interaction pour ComparisonWindow.xaml
    /// </summary>
    public partial class ComparisonWindow : Window
    {
        FHXCompareResultList results = null;

        public ComparisonWindow()
        {
            InitializeComponent();
        }

        private void ExportExcel(object sender, RoutedEventArgs e)
        {
            if (results == null) return;
            string sMessageBoxText = string.Format("Exporter la comparaison ?");
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
                        FHXExcelExporter.ExportComparison(results, saveFileDialog.FileName);
                    break;
            }
        }

        private void Compare(object sender, RoutedEventArgs e)
        {
            string n1 = "";
            string n2 = "";

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Open the first FHX File";
            if (openFileDialog1.ShowDialog() == true)
                n1 = openFileDialog1.FileName;

            OpenFileDialog openFileDialog2 = new OpenFileDialog();
            openFileDialog2.Title = "Open the second FHX File";
            if (openFileDialog2.ShowDialog() == true)
                n2 = openFileDialog2.FileName;

            if (n1 == "" || n2 == "")
            {
                return;
            }

            FHXObject a = FHXObject.FromFile(n1);
            //a = a.Children.Single(i => i.Type == "MODULE");

            FHXObject b = FHXObject.FromFile(n2);
            //b = b.Children.Single(i => i.Type == "MODULE");

            if (a == null || b == null)
            {
                MessageBox.Show("Error loading one of the two objects");
                return;
            }

            double coef = 37.0 / 11940138.0;
            double sec = coef * (double)(a.GetAllParameters().Count * b.GetAllParameters().Count);
            string unit = "s";
            if (sec > 60.0)
            {
                sec = sec / 60.0;
                unit = "m";
            }
            else if (sec > 3600.0)
            {
                sec = sec / 3600.0;
                unit = "h";
            }


            string sMessageBoxText = string.Format("La procédure prendra environ {0}{1}. Continuer ?", sec, unit);
            string sCaption = "Valider la comparaison";

            MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
            MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

            MessageBoxResult rsltMessageBox = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

            switch (rsltMessageBox)
            {
                case MessageBoxResult.Yes:
                    results = FHXComparator.CompareObjects(a, b);
                    this.gridResults.ItemsSource = results.Results;
                    this.gridResults.Columns[1].Visibility = Visibility.Hidden;
                    break;

                case MessageBoxResult.No:
                    return;

                case MessageBoxResult.Cancel:
                    return;
            }
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            // execute some code
            DataGridRow r = (DataGridRow)sender;
            dynamic i = r.Item;
            List<FHXCompareResult> rs = i.Value;
            this.gridResultsValue.ItemsSource = rs;
        }
    }
}
