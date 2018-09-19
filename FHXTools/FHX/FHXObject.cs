using FHXTools.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FHXTools.FHX
{
    public class FHXObject
    {
        public List<FHXObject> Children = new List<FHXObject>();
        public List<FHXParameter> Parameters = new List<FHXParameter>();

        public FHXObject Parent { get; set; }

        public string Type { get; set; }
        public string Name { get; set; }

        public FHXObject()
        {
            this.Name = "";
        }

        public FHXObject(string type, string name = "")
        {
            this.Name = name;
            this.Type = type;
        }

        public override string ToString()
        {
            return this.Path();
        }

        public void AddChild(FHXObject obj)
        {
            Children.Add(obj);
            obj.SetParent(this);
        }

        public void RemoveChild(FHXObject obj)
        {
            Children.Remove(obj);
        }

        public void AddParameter(FHXParameter p)
        {
            Parameters.Add(p);
            p.SetParent(this);
        }

        public void SetParent(FHXObject parent)
        {
            if(parent == null && this.Parent != null)
            {
                this.Parent.RemoveChild(this);
            }
            this.Parent = parent;
        }

        public string GetName()
        {
            if (this.Name != "" && this.Name != null) return this.Name;

            if(this.Type == "SIMPLE_IO_CHANNEL")
            {
                return "CH" + this.GetParameter("POSITION").Value;
            }
            else if (this.Type == "SERIAL_IO_PORT")
            {
                return string.Format("P{0}", this.GetParameter("POSITION").Value);
            }
            else if (this.Type == "SERIAL_IO_DEVICE")
            {
                return string.Format("I{0}", this.GetParameter("INDEX").Value);
            }
            else if(this.Type == "SIMPLE_IO_CARD" || this.Type == "SERIAL_IO_CARD")
            {
                return string.Format("{0}/C{1}", this.GetParameter("CONTROLLER").Value, this.GetParameter("CARD_SLOT").Value);
            }
            else
            {
                bool hasName = this.Parameters.Any(s => s.Identifier == "NAME" || s.Identifier == "TAG");

                if (hasName)
                {

                    FHXParameter p = this.Parameters.Single(s => s.Identifier == "NAME" || s.Identifier == "TAG");
                    return p.Value;
                }
                else
                {
                    return this.Type;
                }
            }
        }

        public FHXParameter GetParameter(string name)
        {
            bool has = HasParameter(name);
            return has ? this.Parameters.Single(s => s.Identifier == name) : null;
        }

        public string Path()
        {
            string s = "";
            if(Parent != null)
            {
                s += Parent.Path();
            }
            s += @"/" + this.GetName();
            return s;
        }

        public FHXObject GetRoot()
        {
            return (this.Parent == null) ? this : this.Parent.GetRoot();
        }

        public TreeViewItem ToTreeViewItem(bool recursive)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = this.GetName();
            item.Tag = this;

            if (recursive)
            {
                IEnumerable<FHXObject> query = Children.OrderBy(i => i.GetName());
                foreach (FHXObject o in query)
                {
                    item.Items.Add(o.ToTreeViewItem(true));
                }
            }
            
            return item;
        }

        public List<FHXObject> GetAllChildren()
        {
            List<FHXObject> c = new List<FHXObject>();
            c.AddRange(this.Children);

            foreach(FHXObject o in Children)
            {
                c.AddRange(o.GetAllChildren());
            }

            return c;
        }

        public List<FHXParameter> GetAllParameters()
        {
            List<FHXObject> c = GetAllChildren();
            List<FHXParameter> p = new List<FHXParameter>();

            p.AddRange(this.Parameters);

            foreach (FHXObject o in c)
            {
                p.AddRange(o.Parameters);
            }

            return p;
        }

        public bool HasParameter(string name)
        {
            return this.Parameters.Any(i => i.Identifier == name);
        }

        public FHXObject GetChild(string name, bool deep = false)
        {
            List<FHXObject> c = deep ? GetAllChildren() : Children;
            return c.Any(i => i.GetName() == name) ? c.Single(i => i.GetName() == name) : null;
        }

        public FHXObject GetChildFromPath(string path)
        {
            string[] hierarchy = path.Split('/');
            FHXObject current = this;

            foreach (string p in hierarchy)
            {
                current = current.GetChild(p);

                if (current == null) return null; //Child not found
            }

            return current; // Returns the last child from the search
        }

        public List<FHXSearchResult> Search(string query)
        {
            List<FHXSearchResult> res = new List<FHXSearchResult>();

            //Search Objects
            List<FHXObject> os = GetAllChildren();
            os = os.Where(i => i.Name.Contains(query)).ToList();

            foreach (var o in os)
            {
                res.Add(new FHXSearchResult(o));
            }

            //Search Parameters
            List<FHXParameter> ps = GetAllParameters();
            ps = ps.Where(i => i.Identifier.Contains(query) || i.Value.Contains(query)).ToList();

            foreach (var p in ps)
            {
                res.Add(new FHXSearchResult(p));
            }

            return res;
        }

        public static FHXObject FromFile(string file)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string s = File.ReadAllText(file);
            sw.Stop();
            Console.WriteLine("Reading file took {0} ms", sw.ElapsedMilliseconds);

            FHXObject root = FromString(s);
            root.Name = System.IO.Path.GetFileNameWithoutExtension(file);
            return root;
        }

        public static FHXObject FromString(string s)
        {
            Stopwatch sw = new Stopwatch();
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

            BuildDeltaVHierarchy(root);

            return root;
        }

        public static void BuildDeltaVHierarchy(FHXObject obj)
        {
            FHXObject root = obj.GetRoot();
            List<FHXObject> AllChildren = root.GetAllChildren();


            //Replace FBs with DEFINITION by their definition
            List<FHXObject> FUNCTION_BLOCK = AllChildren.Where(i => i.Type == "FUNCTION_BLOCK" && i.Parameters.Any(j => j.Identifier == "DEFINITION")).ToList();
            List<FHXObject> FUNCTION_BLOCK_DEFINITION = AllChildren.Where(i => i.Type == "FUNCTION_BLOCK_DEFINITION").ToList();
            foreach (FHXObject fb in FUNCTION_BLOCK)
            {
                FHXObject parent = fb.Parent;
                fb.Parent.RemoveChild(fb);
                fb.SetParent(null);

                if (FUNCTION_BLOCK_DEFINITION.Any(i => i.GetName() == fb.GetParameter("DEFINITION").Value))
                {
                    FHXObject newChild = FUNCTION_BLOCK_DEFINITION.Single(i => i.GetName() == fb.GetParameter("DEFINITION").Value);
                    newChild.Name = fb.GetName();
                    newChild.Type = "FUNCTION_BLOCK";
                    newChild.Parent.RemoveChild(newChild);
                    newChild.SetParent(null);
                    parent.AddChild(newChild);
                }
            }

            //Removes the VALUE Objects and sets their parameters to their parent.
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

            //Replace ATTRIBUTE by ATTRIBUTE_INSTANCE when needed
            List<FHXObject> ATTRIBUTE_INSTANCE = AllChildren.Where(i => i.Type == "ATTRIBUTE_INSTANCE").ToList();
            foreach (FHXObject attr in ATTRIBUTE_INSTANCE)
            {
                FHXObject parent = attr.Parent;

                List<FHXObject> ATTRIBUTE = AllChildren.Where(i => i.Type == "ATTRIBUTE" && i.GetName() == attr.GetName()).ToList();

                if(ATTRIBUTE.Count > 1)
                {
                    Console.WriteLine("Multiple instances of {0} found", attr.GetName());
                }

                foreach(var a in ATTRIBUTE)
                {
                    //Adds the ATTRIBUTE_INSTANCE in the parent of the ATTRIBUTE
                    attr.SetParent(null);
                    a.Parent.AddChild(attr);

                    //Removes the ATTRIBUTE from its parent
                    a.Parent.RemoveChild(a);
                    a.SetParent(null);
                }
            }

            //Removes the useless items by type
            List<string> unused = new List<string>() { "WIRE", "GRAPHICS", "CATEGORY", "FUNCTION_BLOCK_TEMPLATE", "FUNCTION_BLOCK_DEFINITION" };
            foreach (string u in unused)
            {
                List<FHXObject> uc = AllChildren.Where(i => i.Type == u).ToList();
                foreach (FHXObject fb in uc)
                {
                    fb.Parent.RemoveChild(fb);
                    fb.SetParent(null);
                }
            }

            //Removes the useless items by name
            unused = new List<string>() { "RECTANGLE", "POSITION", "ORIGIN", "END", "CATEGORY"};
            foreach (string u in unused)
            {
                List<FHXObject> uc = AllChildren.Where(i => i.Name == u).ToList();
                foreach (FHXObject fb in uc)
                {
                    fb.Parent.RemoveChild(fb);
                    fb.SetParent(null);
                }
            }
        }
    }
}
