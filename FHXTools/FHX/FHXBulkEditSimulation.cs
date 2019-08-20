using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace FHXTools.FHX
{
    class FHXBulkEditSimulation
    {
        public static Dictionary<string, List<string>> AreasModules(string file_data)
        {
            Dictionary<string, List<string>> areas = new Dictionary<string, List<string>>();
            Regex r = new Regex("MODULE TAG=\"([a-zA-Z0-9_-]+)\" PLANT_AREA=\"([a-zA-Z0-9_\\-\\/]+)\"");

            foreach(Match m in r.Matches(file_data))
            {
                string Module = m.Groups[1].Value;
                string Area = m.Groups[2].Value.Split('/')[0];

                if (!areas.ContainsKey(Area))
                {
                    areas.Add(Area, new List<string>());
                }
                areas[Area].Add(Module);
            }

            return areas;
        }

        public static void GenerateFiles(Dictionary<string, List<string>> Areas, string folder)
        {
            foreach (string key in Areas.Keys)
            {
                File.WriteAllLines(folder + @"\SIMUL-" + key + ".txt", Areas[key]);
            }
        }
    }
}
