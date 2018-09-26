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
    /// Logique d'interaction pour ReferenceWindow.xaml
    /// </summary>
    public partial class ReferenceWindow : Window
    {
        List<FHXReference> References = null;
        FHXObject Root;

        public ReferenceWindow(FHXObject root)
        {
            this.Root = root;
            InitializeComponent();
            this.Run();
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            Run();
        }

        private void Run()
        {
            List<FHXParameter> parameters = Root.GetAllParameters();
            References = FHXReferenceSearch.Search(parameters);
            this.gridResults.ItemsSource = References;
        }

        private void ExportExcel(object sender, RoutedEventArgs e)
        {
            if (References == null) return;
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
                        FHXExporter.ExportReferences(References, saveFileDialog.FileName);
                    break;
            }
        }
    }
}
