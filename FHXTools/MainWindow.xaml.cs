using FHXTools.FHX;
using FHXTools.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FHXTools
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FHXObject root;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                ChargerFichier((openFileDialog.FileName));
        }

        private void ChargerFichier(string file)
        {
            root = FHXObject.FromFile(file);
            FHXObject.BuildDeltaVHierarchy(root);

            this.tvMain.Items.Add(root.ToTreeViewItem(true));
        }

        private void tvMain_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem i = (TreeViewItem)e.NewValue;
            FHXObject o = (FHXObject)i.Tag;
            this.labelBottom.Content = o.Path();

            this.gridParam.ItemsSource = o.Parameters;
            this.gridParam.Columns[2].Visibility = Visibility.Hidden; //Hides the Parent field
        }

        private void OpenSearchWindow(object sender, RoutedEventArgs e)
        {
            SearchWindow w = new SearchWindow(this.root);
            w.Show();
        }

        private void OpenComparison(object sender, RoutedEventArgs e)
        {
            ComparisonWindow w = new ComparisonWindow();
            w.Show();
        }
    }
}
