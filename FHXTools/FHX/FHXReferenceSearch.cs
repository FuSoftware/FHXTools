using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FHXTools.FHX
{
    public class FHXReferenceSearch
    {
        public static List<FHXReference> Search(List<FHXParameter> parameters)
        {
            List<FHXParameter> sources = parameters.Where(i => i.HasModule).ToList();
            List<FHXReference> results = new List<FHXReference>();

            long counter = 0;
            long max = sources.Count;

            Parallel.For(0, sources.Count, (i) =>
            {
                string dv_path = sources[i].DeltaVPath;
                for (int j=i+1;j< sources.Count; j++)
                {
                    if(sources[j].VariableType == "string")
                    {
                        if (sources[j].Value.Contains(dv_path))
                        {
                            results.Add(new FHXReference("EXTERNAL", sources[i], sources[j]));
                        }
                    }
                }
                Interlocked.Increment(ref counter);
            });
            return results;
        }

        public static List<FHXReference> Search2(List<FHXParameter> parameters)
        {
            List<FHXParameter> sources = parameters.Where(i => i.HasModule).ToList();
            List<FHXReference>[] results = new List<FHXReference>[sources.Count];

            long counter = 0;
            long max = sources.Count;

            Parallel.For(0, sources.Count, (i) =>
            {
                results[i] = Search(sources[i], sources);
                Interlocked.Increment(ref counter);
            });

            List<FHXReference> r = new List<FHXReference>();

            foreach(var lr in results)
            {
                r.AddRange(lr);
            }

            return r;
        }

        public static List<FHXReference> Search(FHXParameter parameter, List<FHXParameter> sources)
        {
            List<FHXReference> results = new List<FHXReference>();
            string dv_path = parameter.DeltaVPath;
            for (int j = 0; j < sources.Count; j++)
            {
                if (sources[j].VariableType == "string" && sources[j] != parameter)
                {
                    if (sources[j].Value.Contains(dv_path))
                    {
                        results.Add(new FHXReference("EXTERNAL", parameter, sources[j]));
                    }
                }
            }
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
