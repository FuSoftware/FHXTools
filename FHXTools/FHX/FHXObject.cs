using FHXTools.Parsing;
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
            return this.Path;
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
                return string.Format("CH{0}", this.GetParameter("POSITION").Value);
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
                bool hasName = this.Parameters.Any(s => s.Name == "NAME" || s.Name == "TAG");

                if (hasName)
                {

                    FHXParameter p = this.Parameters.Single(s => s.Name == "NAME" || s.Name == "TAG");
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
            return has ? this.Parameters.Single(s => s.Name == name) : null;
        }

        public string Path
        {
            get
            {
                string s = "";
                if (Parent != null)
                {
                    s += Parent.Path;
                }
                s += @"/" + this.GetName();
                return s;
            }
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

        public bool HasParameter(string name) => this.Parameters.Any(i => i.Name == name);

        public bool HasChild(string name) => this.Children.Any(i => i.Name == name);

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
            ps = ps.Where(i => i.Name.Contains(query) || i.Value.Contains(query)).ToList();

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
            /*
            while (!ts.EOF())
            {
                Token t = ts.Next();
                if (t != null) tokens.Add(t);
            }
            sw.Stop();
            Console.WriteLine("Tokenizing file took {0} ms", sw.ElapsedMilliseconds);
            */
            sw.Restart();
            Parser p = new TokenStreamParser(ts);
            FHXObject root = p.ParseAll();
            sw.Stop();
            Console.WriteLine("Parsing file took {0} ms", sw.ElapsedMilliseconds);

            FHXHierarchyBuilder.BuildDeltaVHierarchy(root);

            return root;
        }
    }
}
