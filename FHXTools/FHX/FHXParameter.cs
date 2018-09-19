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
        public string Identifier { get; set; }
        public string Value { get; set; }

        public FHXParameter(string identifier, string value, FHXObject parent = null)
        {
            this.Parent = parent;
            this.Identifier = identifier;
            this.Value = value;
        }

        public void SetParent(FHXObject parent = null)
        {
            this.Parent = parent;
        }

        public override string ToString()
        {
            return this.Path;
        }

        public string RelativePath(FHXObject root)
        {
            FHXObject parent = this.Parent;
            string path = this.Identifier;

            while (parent != null && parent != root)
            {
                path = parent.GetName() + "/" + path;
                parent = parent.Parent;
            }

            return path;
        }

        public string Path
        {
            get{ return Parent.Path() + @"/" + this.Identifier; }
        }
    }
}
