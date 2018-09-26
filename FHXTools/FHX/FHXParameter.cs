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
        public string VariableType
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

        public string Type
        {
            get
            {
                return this.Parent.GetParameter("TYPE") == null ? "" : this.Parent.GetParameter("TYPE") .Value;
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
            string path = "";

            while (parent != null && parent != root)
            {
                path = "/" + parent.Name + path;
                parent = parent.Parent;
            }

            return path + "." + this.Name;
        }

        public FHXObject Module
        {
            get
            {
                return HasModule ? this.GetParentFromType("MODULE") : new FHXObject("", "");
            } 
        }

        public bool HasModule
        {
            get
            {
                return this.GetParentFromType("MODULE") != null;
            }
        }

        public string Path
        {
            get{ return Parent.Path + "." + this.Name; }
        }

        public string DeltaVPath
        {
            get { return "/" + this.RelativePath(Module); }
        }
    }
}
