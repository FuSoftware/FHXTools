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
using System.IO;

namespace FHXTools.Views
{
    /// <summary>
    /// Logique d'interaction pour SearchWindow.xaml
    /// </summary>
    public partial class AffectationWindow : Window
    {
        FHXObject root;
        List<FHXSearchResult> results = new List<FHXSearchResult>();

        public AffectationWindow()
        {
            InitializeComponent();
        }

        public AffectationWindow(FHXObject root)
        {
            InitializeComponent();
            this.root = root;
        }

        private void OpenCSV(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string[] affectations = File.ReadAllLines(openFileDialog.FileName);
                var res = FHXAffectationChecker.CheckAffectations(affectations, this.root);

                Console.WriteLine("{0} / {1}", affectations.Length, res.Keys.Count);

                results.Clear();
                foreach (var r in res.Keys)
                {
                    if (res.Keys.Contains(r))
                    {
                        if (res[r].Count == 0)
                        {
                            results.Add(new FHXSearchResult(new FHXParameter(r, "NULL")));
                        }
                    }

                }
                this.gridSearch.ItemsSource = results;
                this.gridSearch.Columns[0].Visibility = Visibility.Hidden; //Hides the Parent field
                this.gridSearch.Columns[1].Visibility = Visibility.Hidden; //Hides the Parent field
            }            
        }

        private void ExportExcel(object sender, RoutedEventArgs e)
        {
            if (results == null) return;
            string sMessageBoxText = string.Format("Exporter la recherche ?");
            string sCaption = Properties.Resources.Export;

            MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
            MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

            MessageBoxResult rsltMessageBox = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

            switch (rsltMessageBox)
            {
                case MessageBoxResult.Yes:
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Excel file (*.xlsx)|*.xlsx";
                    if (saveFileDialog.ShowDialog() == true)
                        FHXExporter.ExportRecherche(results, saveFileDialog.FileName);
                    break;
            }
        }
    }
}
