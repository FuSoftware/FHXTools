using FHXTools.Parsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FHXTools.FHX
{
    public class FHXParserWrapper
    {
        public static float ParsingPercent
        {
            get
            {
                return mParser == null ? 0 : mParser.mTokenStream.Input.DonePercent;
            }
        }
        public static string State { get; set; }
        public static float BuildingPercent { get; set; }

        private static TokenStreamParser mParser = null;

        static public FHXObject FromFile(string file)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string s = File.ReadAllText(file);
            sw.Stop();
            Console.WriteLine("Reading file took {0} ms", sw.ElapsedMilliseconds);

            FHXObject root = FromString(s);
            root.mName = Path.GetFileNameWithoutExtension(file);
            return root;
        }

        static public FHXObject FromString(string s)
        {
            BuildingPercent = 0;
            State = Properties.Resources.ParsingParsing;

            Stopwatch sw = new Stopwatch();
            sw.Restart();
            TokenStream ts = new TokenStream(s);
            sw.Restart();
            mParser = new TokenStreamParser(ts);
            FHXObject root = mParser.ParseAll();
            sw.Stop();
            Console.WriteLine("Parsing file took {0} ms", sw.ElapsedMilliseconds);

            mParser.mTokenStream.Input.Clear();

            return root;
        }

        static public void BuildDeltaVHierarchy(FHXObject obj)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            FHXObject root = obj.GetRoot();
            List<FHXObject> AllChildren = root.GetAllChildren();

            //Replace FBs with DEFINITION by their definition
            State = Properties.Resources.ParsingClasses;
            BuildingPercent = 25;
            List<FHXObject> FUNCTION_BLOCK = AllChildren.Where(i => i.Type == "FUNCTION_BLOCK" && i.Parameters.Any(j => j.Name == "DEFINITION")).ToList();
            List<FHXObject> FUNCTION_BLOCK_DEFINITION = AllChildren.Where(i => i.Type == "FUNCTION_BLOCK_DEFINITION").ToList();
            
            foreach (FHXObject fb in FUNCTION_BLOCK)
            {
                ProcessFB(fb, FUNCTION_BLOCK_DEFINITION);
            }
            /*
            Parallel.ForEach(FUNCTION_BLOCK, fb =>
            {
                ProcessFB(fb, FUNCTION_BLOCK_DEFINITION);
            });
            */
            sw.Stop();
            Console.WriteLine("{0} took {1}ms", "Loading classes", sw.ElapsedMilliseconds);

            //Removes the VALUE Objects and sets their parameters to their parent.
            State = Properties.Resources.ParsingValues;
            BuildingPercent = 40;
            sw.Restart();
            List<FHXObject> VALUE = AllChildren.Where(i => i.Type == "VALUE").ToList();
            foreach (FHXObject fb in VALUE)
            {
                FHXObject parent = fb.Parent;

                foreach (FHXParameter p in fb.Parameters)
                {
                    parent.AddParameter(p);
                    fb.SetParent(null);
                }
            }
            sw.Stop();
            Console.WriteLine("{0} took {1}ms", "Removing VALUEs", sw.ElapsedMilliseconds);


            //Replace ATTRIBUTE by ATTRIBUTE_INSTANCE when needed
            State = Properties.Resources.ParsingAttributes;
            BuildingPercent = 75;
            sw.Restart();
            List<FHXObject> ATTRIBUTE_INSTANCE = AllChildren.Where(i => i.Type == "ATTRIBUTE_INSTANCE").ToList();
            foreach (FHXObject attr in ATTRIBUTE_INSTANCE)
            {
                ProcessAttribute(attr);
            }
            /*
            Parallel.ForEach(ATTRIBUTE_INSTANCE, attr =>
            {
                ProcessAttribute(attr);
            });
            */
            sw.Stop();
            Console.WriteLine("{0} took {1}ms", "Replacing ATTRIBUTEs", sw.ElapsedMilliseconds);

            //Removes the useless items by type
            State = Properties.Resources.ParsingCleaning;
            BuildingPercent = 90;
            sw.Restart();
            List<string> unused_types = new List<string>() { "WIRE", "GRAPHICS", "FUNCTION_BLOCK_TEMPLATE", "FUNCTION_BLOCK_DEFINITION" };
            List<string> unused_names = new List<string>() { "RECTANGLE", "POSITION", "ORIGIN", "END" };
            List<FHXObject> uc = AllChildren.Where(i => unused_types.Contains(i.Type) || unused_names.Contains(i.mName)).ToList();
            foreach (FHXObject fb in uc)
            {
                fb.Parent.RemoveChild(fb);
                fb.SetParent(null);
            }
            sw.Stop();
            Console.WriteLine("{0} took {1}ms", "Clearing hierarchy", sw.ElapsedMilliseconds);
            State = "Done";
            BuildingPercent = 100;
        }

        private static void ProcessFB(FHXObject fb, List<FHXObject> FUNCTION_BLOCK_DEFINITION)
        {
            FHXObject parent = fb.Parent;
            fb.Parent.RemoveChild(fb);
            fb.SetParent(null);

            if (FUNCTION_BLOCK_DEFINITION.Any(i => i.Name == fb.GetParameter("DEFINITION").Value))
            {
                foreach (FHXObject newChild in FUNCTION_BLOCK_DEFINITION.Where(i => i.Name == fb.GetParameter("DEFINITION").Value))
                {
                    newChild.mName = fb.Name;
                    newChild.Type = "FUNCTION_BLOCK";
                    newChild.Parent.RemoveChild(newChild);
                    newChild.SetParent(null);
                    parent.AddChild(newChild);
                }
            }
        }

        private static void ProcessAttribute(FHXObject attr)
        {
            FHXObject parent = attr.Parent;

            if (parent == null) return;
            List<FHXObject> ATTRIBUTE = parent.GetAllChildren();
            ATTRIBUTE = ATTRIBUTE.Where(i => i.Type == "ATTRIBUTE" && i.Name == attr.Name).ToList();

            if (ATTRIBUTE.Count > 1)
            {
                Console.WriteLine("Multiple instances of {0} found", attr.Name);
            }

            foreach (var a in ATTRIBUTE)
            {
                if (a.Parent != null)
                {
                    //Adds the ATTRIBUTE_INSTANCE in the parent of the ATTRIBUTE
                    attr.SetParent(null);
                    a.Parent.AddChild(attr);

                    //Removes the ATTRIBUTE from its parent
                    a.Parent.RemoveChild(a);
                    a.SetParent(null);
                }
            }
        }
    }
}
