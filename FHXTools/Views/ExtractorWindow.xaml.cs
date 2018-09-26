using CSScriptLibrary;
using FHXTools.FHX;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Logique d'interaction pour Extractor.xaml
    /// </summary>
    public partial class ExtractorWindow : Window
    {
        FHXObject root = null;
        List<FHXParameter> results;

        public ExtractorWindow(FHXObject root)
        {
            this.root = root;
            InitializeComponent();
        }

        private void Run(object sender, RoutedEventArgs e)
        {
            try
            {
                dynamic script = CSScript.Evaluator.LoadMethod(tbScript.Text);
                results = FHXParameterExtractor.ExtractPattern(root, script);
                this.gridResults.ItemsSource = results;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while loading script : " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void ExportExcel(object sender, RoutedEventArgs e)
        {
            if (results == null) return;
            string sMessageBoxText = string.Format("Exporter la comparaison ?");
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
                        FHXExporter.ExportParameterList(results, saveFileDialog.FileName);
                    break;
            }
        }

        private void ImportRoutine(object sender, RoutedEventArgs e)
        {
            string file = "";

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = Properties.Resources.OpenFirstFile;
            if (openFileDialog1.ShowDialog() == true)
                file = openFileDialog1.FileName;

            if (file == "") return;

            tbScript.Text = File.ReadAllText(file);
        }

        private void ExportRoutine(object sender, RoutedEventArgs e)
        {
            string file = "";

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "C# Script (*.cs)|*.cs";
            if (saveFileDialog.ShowDialog() == true)
                file = saveFileDialog.FileName;

            if (file == "") return;

            File.WriteAllText(file, tbScript.Text);
        }
    }
}
