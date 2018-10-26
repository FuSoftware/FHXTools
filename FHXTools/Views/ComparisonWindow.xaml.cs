using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
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
                        FHXExporter.ExportComparison(results, saveFileDialog.FileName);
                    break;
            }
        }

        private void Compare(object sender, RoutedEventArgs e)
        {
            string n1 = "";
            string n2 = "";

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = Properties.Resources.OpenFirstFile;
            if (openFileDialog1.ShowDialog() == true)
                n1 = openFileDialog1.FileName;

            OpenFileDialog openFileDialog2 = new OpenFileDialog();
            openFileDialog2.Title = Properties.Resources.OpenSecondFile;
            if (openFileDialog2.ShowDialog() == true)
                n2 = openFileDialog2.FileName;

            if (n1 == "" || n2 == "")
            {
                return;
            }

            FHXObject a = FHXParserWrapper.FromFile(n1);
            //a = a.Children.Single(i => i.Type == "MODULE");

            FHXObject b = FHXParserWrapper.FromFile(n2);
            //b = b.Children.Single(i => i.Type == "MODULE");

            if (a == null || b == null)
            {
                MessageBox.Show(Properties.Resources.ErrorLoading);
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
            FHXCompareResult rs = i.Value;

            var diffBuilder = new SideBySideDiffBuilder(new Differ());

            if(rs.NewValue == null)
            {
                this.tbOld.Document.Blocks.Clear();
                this.tbNew.Document.Blocks.Clear();
                
                var p = new Paragraph();
                var tr = new TextRange(p.ContentStart, p.ContentEnd);
                tr.Text = rs.OldValue;

                this.tbOld.Document.Blocks.Add(p);
            }
            else
            {
                var diff = diffBuilder.BuildDiffModel(rs.OldValue, rs.NewValue);

                RichTextBox[] tb = new RichTextBox[2] { this.tbOld, this.tbNew };
                List<DiffPiece>[] t = new List<DiffPiece>[2] { diff.OldText.Lines, diff.NewText.Lines };

                for (int j = 0; j < 2; j++)
                {
                    tb[j].Document.Blocks.Clear();
                    foreach (DiffPiece line in t[j])
                    {
                        var p = new Paragraph();
                        p.Margin = new Thickness(0);
                        TextRange tr;
                        tr = new TextRange(p.ContentStart, p.ContentEnd);
                        switch (line.Type)
                        {
                            case ChangeType.Inserted:
                                tr.Text = line.Text;
                                tr.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.LightGreen);
                                break;
                            case ChangeType.Deleted:
                                tr.Text = line.Text;
                                tr.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.LightPink);
                                break;
                            case ChangeType.Modified:
                                tr.Text = line.Text;
                                tr.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Goldenrod);
                                break;
                            case ChangeType.Imaginary:
                                tr.Text = "            ";
                                tr.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Gray);
                                break;
                            case ChangeType.Unchanged:
                                tr.Text = line.Text;
                                tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);
                                break;
                            default:
                                tr.Text = line.Text;
                                tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.White);
                                break;
                        }
                        tb[j].Document.Blocks.Add(p);
                    }
                }
            }
            
        }
    }
}
