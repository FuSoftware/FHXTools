using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace FHXTools.FHX.Conversion
{
    class FHXJsonConverter
    {

        public static FHXObject FromJson(string file)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            dynamic oJson = JsonConvert.DeserializeObject(File.ReadAllText(file));
            FHXObject o = FromJsonObject(oJson);
            sw.Stop();
            Console.WriteLine("Loaded {0} in {1}ms", file, sw.ElapsedMilliseconds);
            return o;
        }
        
        public static FHXObject FromJsonObject(dynamic j)
        {
            FHXObject o = new FHXObject();
            o.Type = j.Type;

            if(j.Parameters != null)
            {
                foreach (dynamic p in j.Parameters)
                {
                    o.Parameters.Add(FromJsonParameter(p));
                }
            }
            
            if(j.Children != null)
            {
                foreach (dynamic c in j.Children)
                {
                    o.Children.Add(FromJsonObject(c));
                }
            }

            return o;
        }

        public static FHXParameter FromJsonParameter(dynamic j)
        {
            string name = j.Name;
            string value = j.Value;
            bool mandatory = j.Mandatory;
            return new FHXParameter(name, value, mandatory);
        }


        public static void ToJson(FHXObject root, string file)
        {
            File.WriteAllText(file, ToJsonObject(root).ToString(Formatting.None));
        }

        public static JObject ToJsonObject(FHXObject o)
        {
            dynamic j = new JObject();
            j.Type = o.Type;

            if(o.Parameters.Count > 0)
            {
                JArray a = new JArray();
                foreach (FHXParameter p in o.Parameters)
                {
                    a.Add(ToJsonParameter(p));
                }
                j.Parameters = a;
            }

            if(o.Children.Count > 0)
            {
                JArray a = new JArray();
                foreach (FHXObject c in o.Children)
                {
                    a.Add(ToJsonObject(c));
                }
                j.Children = a;
            }
            
            return j;
        }

        public static JObject ToJsonParameter(FHXParameter p)
        {
            dynamic j = new JObject();
            j.Name = p.Name;
            j.Value = p.Value;
            j.Mandatory = p.Mandatory;
            return j;
        }
    }
}
