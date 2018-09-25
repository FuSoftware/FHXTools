using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHXTools.FHX
{
    class FHXParameterExtractor
    {

        public static List<string> ExtractList(FHXObject root, List<string> path)
        {
            return ExtractList(root.GetAllParameters(), path);
        }

        public static List<string> ExtractList(List<FHXParameter> parameters, List<string> paths)
        {
            List<string> results = new List<string>();

            foreach(string p in paths)
            {
                results.Add(Extract(parameters, p));
            }

            return results;
        }

        public static string Extract(FHXObject root, string path)
        {
            return Extract(root.GetAllParameters(), path);
        }

        public static string Extract(List<FHXParameter> parameters, string path)
        {
            if (parameters.Any(i => i.Path == path))
            {
                return parameters.Single(i => i.Path == path).Value;
            }
            else
            {
                return "";
            }
        }
    }
}
