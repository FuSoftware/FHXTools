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

        public string Name { get; set; }
        public string Type { get; set; }

        public FHXObject()
        {
        }

        public FHXObject(string type, string name)
        {
            this.Type = type;
            this.Name = name;
        }

        public void AddChild(FHXObject obj)
        {
            Children.Add(obj);
            obj.SetParent(this);
        }

        public void AddParameter(FHXParameter p)
        {
            Parameters.Add(p);
            p.SetParent(this);
        }

        public void SetParent(FHXObject parent)
        {
            this.Parent = parent;
        }

        public string FindName()
        {
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

        public string Path()
        {
            string s = "";
            if(Parent != null)
            {
                s += Parent.Path();
            }
            s += @"\" + this.FindName();
            return s;
        }

        public TreeViewItem ToTreeViewItem(bool recursive)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = this.FindName();
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
    }
}
