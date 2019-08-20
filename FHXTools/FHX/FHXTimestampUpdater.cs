using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace FHXTools.FHX
{
    class FHXTimestampUpdater
    {
        public FHXTimestampUpdater()
        {

        }

        public static string UpdateText(string text)
        {
            Regex r = new Regex(@"time=([0-9]+)");
            List<string> tsp = new List<string>();

            foreach(Match m in r.Matches(text))
            {
                string t = m.Groups[1].Value;
                if (!tsp.Contains(t))tsp.Add(t);
            }

            foreach(string t in tsp)
            {
                text = text.Replace("time=" + t, "time=" + (int.Parse(t) + 1).ToString());
            }

            return text;
        }

        public static void UpdateFile(string path)
        {
            string data = File.ReadAllText(path);
            data = UpdateText(data);
            File.WriteAllText(path, data);
        }
    }
}
