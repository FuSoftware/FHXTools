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
        FHXObject Root;
        FHXObject Selected = null;

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
            Root = FHXObject.FromFile(file);
            FHXHierarchyBuilder.BuildDeltaVHierarchy(Root);

            this.tvMain.Items.Add(Root.ToTreeViewItem(true));
        }

        private void tvMain_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem i = (TreeViewItem)e.NewValue;
            Selected = (FHXObject)i.Tag;
            this.labelBottom.Content = Selected.Path();

            this.gridParam.ItemsSource = Selected.Parameters;
            this.gridParam.Columns[2].Visibility = Visibility.Hidden; //Hides the Parent field
        }

        private void OpenSearchWindow(object sender, RoutedEventArgs e)
        {
            SearchWindow w = new SearchWindow(this.Root);
            w.Show();
        }

        private void OpenComparison(object sender, RoutedEventArgs e)
        {
            ComparisonWindow w = new ComparisonWindow();
            w.Show();
        }

        private void ExportExcel(object sender, RoutedEventArgs e)
        {
            if (Selected == null) return;
            string sMessageBoxText = string.Format("Exporter l'objet {0} ?", Selected.Path());
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
                        FHXExporter.ExportParameters(Selected, saveFileDialog.FileName);
                    break;
            }            
        }

        private void ExportWord(object sender, RoutedEventArgs e)
        {
            if (Selected == null) return;
            string sMessageBoxText = string.Format("Exporter l'objet {0} ?", Selected.Path());
            string sCaption = "Export";

            MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
            MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

            MessageBoxResult rsltMessageBox = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

            switch (rsltMessageBox)
            {
                case MessageBoxResult.Yes:
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Word file (*.docx)|*.docx";
                    if (saveFileDialog.ShowDialog() == true)
                        FHXExporter.ExportObjectWord(Selected, saveFileDialog.FileName);
                    break;
            }
        }

        private void ExportFHX(object sender, RoutedEventArgs e)
        {
            if (Selected == null) return;
            string sMessageBoxText = string.Format("Exporter l'objet {0} ?", Selected.Path());
            string sCaption = "Export";

            MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
            MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

            MessageBoxResult rsltMessageBox = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

            switch (rsltMessageBox)
            {
                case MessageBoxResult.Yes:
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "FHX file (*.fhx)|*.fhx";
                    if (saveFileDialog.ShowDialog() == true)
                        FHXExporter.ExportFHXToFile(Selected, saveFileDialog.FileName);
                    break;
            }
        }
    }
}
