using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHXTools.FHX
{
    public class FHXReferenceSearch
    {
        public static List<FHXReference> Search(List<FHXParameter> parameters)
        {
            List<FHXParameter> sources = parameters.Where(i => i.HasModule).ToList();
            List<FHXReference> results = new List<FHXReference>();

            /*
            foreach (var p1 in sources)
            {
                string dv_path = p1.DeltaVPath;
                foreach (var p2 in parameters.Where(i => i.VariableType == "string" && i.Value.Contains(dv_path)))
                {
                    results.Add(new FHXReference("EXTERNAL", p1, p2));
                }
            }
            */
            Parallel.ForEach(sources, (p1) =>
            {
                string dv_path = p1.DeltaVPath;
                foreach (var p2 in parameters.Where(i => i.VariableType == "string" && i.Value.Contains(dv_path)))
                {
                    results.Add(new FHXReference("EXTERNAL", p1, p2));
                }
            });

            return results;
        }
    }

    public class FHXReference
    {
        public string Type { get; set; } //Internal / External
        public FHXParameter Source { get; set; }
        public FHXParameter Usage { get; set; }

        public FHXReference(string type, FHXParameter source, FHXParameter usage)
        {
            this.Type = type;
            this.Source = source;
            this.Usage = usage;
        }
    }
}
