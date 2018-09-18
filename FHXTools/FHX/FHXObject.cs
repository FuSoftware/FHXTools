using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHXTools.FHX
{
    class FHXObject
    {
        List<FHXObject> Children = new List<FHXObject>();
        FHXObject Parent;

        string Name { get; set; }
        string Type { get; set; }

        public FHXObject(string type, string name)
        {
            this.Type = type;
            this.Name = name;
        }
    }
}
