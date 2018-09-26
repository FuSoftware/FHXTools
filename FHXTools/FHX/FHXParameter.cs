using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHXTools.FHX
{
    public class FHXParameter
    {
        public FHXObject Parent { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public bool Mandatory { get; set; }
        public string Type
        {
            get
            {
                string v = this.Value;
                int p;
                if(v == "F" || v == "T")
                {
                    return "bool";
                }
                else if(int.TryParse(v, out p))
                {
                    return "number";
                }
                else
                {
                    return "string";
                }
            }
        }

        public FHXParameter(string identifier, string value, bool mandatory = false, FHXObject parent = null)
        {
            this.Parent = parent;
            this.Name = identifier;
            this.Value = value;
            this.Mandatory = mandatory;
        }

        public void SetParent(FHXObject parent = null)
        {
            this.Parent = parent;
        }

        public FHXObject GetParentFromType(string type)
        {
            return this.Parent.GetParentFromType(type);
        }

        public override string ToString()
        {
            return this.Path;
        }

        public string RelativePath(FHXObject root)
        {
            FHXObject parent = this.Parent;
            string path = this.Name;

            while (parent != null && parent != root)
            {
                path = parent.GetName() + "/" + path;
                parent = parent.Parent;
            }

            return path;
        }

        public string Path
        {
            get{ return Parent.Path + @"/" + this.Name; }
        }
    }
}
