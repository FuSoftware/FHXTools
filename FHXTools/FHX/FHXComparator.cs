using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHXTools.FHX
{
    class FHXComparator
    {
        public static FHXCompareResultList CompareObjects(FHXObject a, FHXObject b)
        {
            FHXCompareResultList res = new FHXCompareResultList();

            List<FHXParameter> psa = a.GetAllParameters();
            List<FHXParameter> psb = b.GetAllParameters();

            foreach (var pa in psa)
            {
                string rpath = pa.RelativePath(a);
                //Check if b contains the parameter
                if (psb.Any(i => i.RelativePath(b) == rpath))
                {
                    //If it contains it
                    //FHXParameter pb = psb.Single(i => i.RelativePath(b) == rpath);
                    FHXParameter pb = psb.Where(i => i.RelativePath(b) == rpath).ToArray()[0];
                    if (pa.Value != pb.Value)
                    {
                        res.Add(a, pa, FHXCompareType.DIFFERENT);
                        res.Add(b, pb, FHXCompareType.DIFFERENT);
                    }
                }
                else
                {
                    //If not
                    res.Add(a, pa, FHXCompareType.IN);
                }
            }

            foreach (var pb in psb)
            {
                string rpath = pb.RelativePath(b);
                //Check if b contains the parameter
                if (!psb.Any(i => i.RelativePath(b) == rpath))
                {
                    //If not
                    res.Add(b, pb, FHXCompareType.IN);
                }
            }

            return res;
        }
    }
}
