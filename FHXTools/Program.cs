using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using FHXTools.FHX;
using Xceed.Words.NET;
using System.Net;
using FHXTools.Parsing;
using Microsoft.Win32;
using static FHXTools.FHX.FHXStructureHandler;
using FHXTools.FHX;
using FHXTools.FHX.Conversion;

namespace FHXTools
{
    class Program
    {
        public static void Main()
        {
            SearchAffectations();
            Console.ReadLine();
        }

        public static void SearchAffectations()
        {
            string[] affectations = File.ReadAllLines(@"D:\FHX\Affectations.txt");
            FHXObject Root = FHXXMLConverter.FromXML(@"D:\FHX\Base LLIC3 19-08-14.xml");
            var res = FHXAffectationChecker.CheckAffectations(affectations, Root);

            Console.WriteLine("{0} / {1}", affectations.Length, res.Keys.Count);

            foreach(var r in res.Keys)
            {
                if (res.Keys.Contains(r))
                {
                    if (res[r].Count == 0)
                    {
                        Console.WriteLine(r);
                    }
                }
                
            }
        }

        public static void ExtractModules(string file)
        {
            var Areas = FHXBulkEditSimulation.AreasModules(File.ReadAllText(file));
            FHXBulkEditSimulation.GenerateFiles(Areas, @"D:\FHX\Simul");
        }

        public static void TestWord(string file)
        {
            using (var doc = DocX.Load(file))
            {
                
                //doc.Save();
            }
        }

        public static void UpdateTimestamps(string file)
        {
            FHXTimestampUpdater.UpdateFile(file);
        }

        public static void TestTokensFile(string input)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string s = File.ReadAllText(input);
            sw.Stop();
            Console.WriteLine("Reading file took {0} ms", sw.ElapsedMilliseconds);

            sw.Restart();
            List<Token> tokens = TestTokens(s);
            sw.Stop();
            Console.WriteLine("Tokenizing file took {0} ms", sw.ElapsedMilliseconds);

            TestTokenHierarchy(tokens);

            sw.Restart();
            FHXObject root = ParseTokens(tokens);
            sw.Stop();
            Console.WriteLine("Parsing file took {0} ms", sw.ElapsedMilliseconds);
        }

        public static void TestTokenHierarchy(List<Token> tokens)
        {
            long count = 0;
            foreach(Token t in tokens)
            {
                if(t.Type == TokenType.OPEN_BRACKET)
                {
                    count++;
                }
                else if (t.Type == TokenType.CLOSE_BRACKET)
                {
                    count--;
                }
            }

            Console.WriteLine("Hierarchy control, should return zero : {0}", count);
        }

        public static FHXObject ParseTokens(List<Token> tokens)
        {
            Parser p = new TokenListParser(tokens);
            FHXObject o = p.ParseAll();
            return o;
        }

        public static List<Token> TestTokens(string input)
        {
            List<Token> tokens = new List<Token>();
            TokenStream ts = new TokenStream(input);

            while (!ts.EOF())
            {
                Token t = ts.Next();
                if (t != null) tokens.Add(t);
            }
            
            foreach(Token t in tokens)
            {
                //Console.WriteLine(t.ToString());
            }

            return tokens;
        }

        public static void TestNodes()
        {
            //Load File
            string file = @"D:\FHX\SITE_COMMUNS.fhx";
            FHXObject Root = FHXParserWrapper.FromFile(file);
            FHXParserWrapper.BuildDeltaVHierarchy(Root);

            //Test Areas
            FHXStructureHandler h = new FHXStructureHandler(Root);
            FHXNode r = new FHXNode("Root");
            foreach (FHXNode n in h.Areas)
            {
                r.Children.Add(n);
                foreach (FHXNode m in h.ModulesArea(n.Label))
                {
                    n.Children.Add(m);
                }
            }

            Console.WriteLine(r.Label);
        }

    }
}
