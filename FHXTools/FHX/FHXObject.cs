using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FHXTools.FHX
{
    class FHXObject
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

            bool hasName = this.Parameters.Any(s => s.Identifier == "NAME" || s.Identifier == "TAG");

            if(hasName)
            {
                FHXParameter p = this.Parameters.Single(s => s.Identifier == "NAME" || s.Identifier == "TAG");
                return p.Value;
            }
            else
            {
                return this.Type;
            }
        }

        public FHXParameter GetParameter(string name)
        {
            bool has = this.Parameters.Any(s => s.Identifier == name);
            return has ? this.Parameters.Single(s => s.Identifier == name) : null;
        }

        public string Path()
        {
            string s = "";
            if(Parent != null)
            {
                s += Parent.Path();
            }
            s += @"\" + this.GetName();
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
                foreach (FHXObject o in Children)
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

            foreach (FHXObject o in Children)
            {
                p.AddRange(o.Parameters);
            }

            return p;
        }

        public List<FHXSearchResult> Search(string query)
        {
            List<FHXSearchResult> res = new List<FHXSearchResult>();

            //Search Objects
            List<FHXObject> os = GetAllChildren().Where(i => i.Name.Contains(query)).ToList();

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

        public static void BuildDeltaVHierarchy(FHXObject obj)
        {
            FHXObject root = obj.GetRoot();
            List<FHXObject> AllChildren = root.GetAllChildren();

            List<FHXObject> FUNCTION_BLOCK = AllChildren.Where(i => i.Type == "FUNCTION_BLOCK" && i.Parameters.Any(j => j.Identifier == "DEFINITION")).ToList();
            List<FHXObject> FUNCTION_BLOCK_DEFINITION = AllChildren.Where(i => i.Type == "FUNCTION_BLOCK_DEFINITION").ToList();
            List<FHXObject> ATTRIBUTE_INSTANCE = AllChildren.Where(i => i.Type == "ATTRIBUTE_INSTANCE").ToList();
            List<FHXObject> ATTRIBUTE = AllChildren.Where(i => i.Type == "ATTRIBUTE").ToList();
            List<FHXObject> VALUE = AllChildren.Where(i => i.Type == "VALUE").ToList();

            //Replace FBs with DEFINITION by their definition
            foreach (FHXObject fb in FUNCTION_BLOCK)
            {
                FHXObject parent = fb.Parent;
                parent.RemoveChild(fb);

                if (FUNCTION_BLOCK_DEFINITION.Any(i => i.GetName() == fb.GetParameter("DEFINITION").Value))
                {
                    FHXObject newChild = FUNCTION_BLOCK_DEFINITION.Single(i => i.GetName() == fb.GetParameter("DEFINITION").Value);
                    newChild.Name = fb.GetName();
                    parent.AddChild(newChild);
                }               
            }

            //Removes the VALUE Objects and sets their parameters to their parent.
            foreach (FHXObject fb in VALUE)
            {
                FHXObject parent = fb.Parent;

                foreach(FHXParameter p in fb.Parameters)
                {
                    parent.AddParameter(p);
                    fb.SetParent(null);
                }
            }

            //Replace ATTRIBUTE by ATTRIBUTE_INSTANCE when needed
            Console.WriteLine("");
        }
    }
}
