using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHXTools.FHX
{
    class FHXParameter
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
    }
}
