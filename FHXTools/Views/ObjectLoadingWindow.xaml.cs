using FHXTools.FHX;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace FHXTools.Views
{
    /// <summary>
    /// Logique d'interaction pour ProgressWindow.xaml
    /// </summary>
    public partial class ObjectLoadingWindow : Window
    {

        public bool Done { get; set; }
        public FHXObject Root { get; set; }

        public ObjectLoadingWindow(string file)
        {
            InitializeComponent();
            Load(file);
        }

        public void Load(string file)
        {
            var thread = new Thread(
                () =>
                {
                    FHXObject root = FHXParserWrapper.FromFile(file);
                    FHXParserWrapper.BuildDeltaVHierarchy(root);
                    RootLoaded(root);
                });
            thread.Start();

            var thread2 = new Thread(
                () =>
                {
                    pbPercent.Maximum = 100;
                    pbPercent.Minimum = 0;

                    while (this.IsActive)
                    {
                        pbPercent.Value = FHXParserWrapper.State == "Parsing" ? FHXParserWrapper.ParsingPercent : FHXParserWrapper.BuildingPercent;
                        labelPercent.Content = FHXParserWrapper.ParsingPercent.ToString() + "%";
                        labelState.Content = FHXParserWrapper.State;
                        Thread.Sleep(100);
                    }
                });
            thread2.Start();
        }

        private void RootLoaded(FHXObject root)
        {
            this.Root = root;
            this.Hide();
        }
    }
}
