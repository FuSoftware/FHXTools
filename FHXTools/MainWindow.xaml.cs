using FHXTools.FHX;
using FHXTools.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

        private void OpenXML(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                ChargerFichierXML((openFileDialog.FileName));
        }

        private void ChargerFichier(string file)
        {
            var t = new Thread(() => 
            { 
                this.Root = FHXParserWrapper.FromFile(file);
                FHXParserWrapper.BuildDeltaVHierarchy(this.Root);
				this.tvMain.Dispatcher.BeginInvoke(new Action(delegate { this.tvMain.Items.Add(this.Root.ToTreeViewItem(true, false)); })); 
            });
            t.Start();
        }

        private void ChargerFichierXML(string file)
        {
            var t = new Thread(() =>
            {
                this.Root = FHXConverter.FromXML(file);
                //FHXParserWrapper.BuildDeltaVHierarchy(this.Root);
                this.tvMain.Dispatcher.BeginInvoke(new Action(delegate { this.tvMain.Items.Add(this.Root.ToTreeViewItem(true, false)); }));
            });
            t.Start();
        }

        private void tvMain_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem i = (TreeViewItem)e.NewValue;
            Selected = (FHXObject)i.Tag;
            this.labelBottom.Content = Selected.Path;

            this.gridParam.ItemsSource = Selected.Parameters;
            this.gridParam.Columns[2].Visibility = Visibility.Hidden; //Hides the Parent field
        }

        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvi = e.OriginalSource as TreeViewItem;
            if (tvi != null)
            {
                foreach(TreeViewItem tvic in tvi.Items)
                {
                    if (tvic.Items.Count == 0)
                    {
                        FHXObject o = (FHXObject)tvic.Tag;

                        foreach (FHXObject child in o.Children)
                        {
                            tvic.Items.Add(child.ToTreeViewItem(true, false));
                        }
                    }
                }
            }
        }

        private void OpenSearchWindow(object sender, RoutedEventArgs e)
        {
            SearchWindow w = new SearchWindow(this.Root);
            w.Show();
        }

        private void OpenReferences(object sender, RoutedEventArgs e)
        {
            ReferenceWindow w = new ReferenceWindow(this.Root);
            w.Show();
        }

        private void OpenComparison(object sender, RoutedEventArgs e)
        {
            ComparisonWindow w = new ComparisonWindow();
            w.Show();
        }

        private void OpenExtractor(object sender, RoutedEventArgs e)
        {
            ExtractorWindow w = new ExtractorWindow(this.Root);
            w.Show();
        }

        private void Convert(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "XML file (*.xml)|*.xml";
                if (saveFileDialog.ShowDialog() == true)
                {
                    var t = new Thread(() =>
                    {
                        FHXObject o = FHXParserWrapper.FromFile(openFileDialog.FileName);
                        FHXParserWrapper.BuildDeltaVHierarchy(o);
                        FHXConverter.ToXML(o, saveFileDialog.FileName);
                        MessageBox.Show("Fichier converti");
                    });
                    t.Start();
                }
            }
        }

        private void ExportExcel(object sender, RoutedEventArgs e)
        {
            if (Selected == null) return;
            string sMessageBoxText = string.Format("Exporter l'objet {0} ?", Selected.Path);
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

        private void CreateModuleDatabase(object sender, RoutedEventArgs e)
        {
            List<FHXObject> modules = Root.GetAllChildren().Where(i => i.Type == "MODULE").ToList();
            //100ms / module

            string sMessageBoxText = string.Format("Créer la BDD Instrum ? (Prendra environ {0} secondes)", modules.Count * 100 / 1000);
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
                    {
                        OpenFileDialog openFileDialog = new OpenFileDialog();
                        openFileDialog.Filter = "CSV file (*.csv)|*.csv";
                        if (openFileDialog.ShowDialog() == true)
                        {
                            Stopwatch sw = new Stopwatch();
                            sw.Start();
                            FHXDatabaseBuilder b = new FHXDatabaseBuilder();
                            b.SetFromFile(openFileDialog.FileName);
                            b.BuildModules(modules, saveFileDialog.FileName);
                            sw.Stop();
                            MessageBox.Show(string.Format("{0} modules exportés en {1}ms ({2}ms/module)", modules.Count, sw.ElapsedMilliseconds, sw.ElapsedMilliseconds / modules.Count));
                        }
                    }   
                    break;
            }
        }

        private void ExportWord(object sender, RoutedEventArgs e)
        {
            if (Selected == null) return;
            string sMessageBoxText = string.Format("Exporter l'objet {0} ?", Selected.Path);
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
            string sMessageBoxText = string.Format("Exporter l'objet {0} ?", Selected.Path);
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
