using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FHXTools.FHX
{
    public class FHXAffectationChecker
    {
        public static List<FHXSearchResult> CheckAffectation(string affectation, FHXObject root, List<FHXObject> os, List<FHXParameter> ps)
        {
            return root.SearchSimple("//" + affectation, os, ps);
        }

        public static Dictionary<string, List<FHXSearchResult>> CheckAffectations(string[] affectations, FHXObject root)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            long counter = 0;
            long max = affectations.Length;
            //max = 26;
            Dictionary<string, List<FHXSearchResult>> results = new Dictionary<string, List<FHXSearchResult>>();
            List<FHXParameter> ps = root.GetAllParameters().Where(p => p.Name == "REF").ToList();

            
            Parallel.For(0, max, (i) =>
            {
                results[affectations[i]] = CheckAffectation(affectations[i], root, null, ps);
                Interlocked.Increment(ref counter);
                if(counter % 25 == 0)
                {
                    Console.WriteLine("Processed {0}/{1}", counter, affectations.Length);
                }
            });

            /*
            for (long i=0; i < max; i++){
                results[affectations[i]] = CheckAffectation(affectations[i], root, null, ps);
                counter++;
                if (counter % 25 == 0)
                {
                    Console.WriteLine("Processed {0}/{1}", counter, affectations.Length);
                }
            };
            */
            sw.Stop();
            Console.WriteLine("Looked up affectations in {0} ms", sw.ElapsedMilliseconds);

            return results;
        }
    }
}
