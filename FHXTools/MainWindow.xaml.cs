using FHXTools.FHX;
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

        private void ChargerFichier(string fichier)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string s = File.ReadAllText(fichier);
            sw.Stop();
            Console.WriteLine("Reading file took {0} ms", sw.ElapsedMilliseconds);

            sw.Restart();
            List<Token> tokens = new List<Token>();
            TokenStream ts = new TokenStream(s);

            while (!ts.EOF())
            {
                Token t = ts.Next();
                if (t != null) tokens.Add(t);
            }
            sw.Stop();
            Console.WriteLine("Tokenizing file took {0} ms", sw.ElapsedMilliseconds);

            sw.Restart();
            Parser p = new Parser(tokens);
            FHXObject root = p.ParseAll();
            sw.Stop();
            Console.WriteLine("Parsing file took {0} ms", sw.ElapsedMilliseconds);

            FHXObject.BuildDeltaVHierarchy(root);

            this.tvMain.Items.Add(root.ToTreeViewItem(true));
        }

        private void tvMain_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem i = (TreeViewItem)e.NewValue;
            FHXObject o = (FHXObject)i.Tag;
            this.labelBottom.Content = o.Path();
        }
    }
}
