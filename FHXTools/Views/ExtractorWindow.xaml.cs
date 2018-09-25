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
        public ExtractorWindow(FHXObject root)
        {
            this.root = root;
            InitializeComponent();
        }

        private void Run(object sender, RoutedEventArgs e)
        {
            //FHXParameterExtractor.ExtractPattern(root, tbScript.Text);
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
