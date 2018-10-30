using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FHXTools.FHX.Conversion
{
    class FHXXMLConverter
    {
        public static FHXObject FromXML(string file)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(file);
            XmlNode root = xmlDoc.FirstChild;
            FHXObject o = FromXMLObject(root.FirstChild);

            sw.Stop();
            Console.WriteLine("Loaded {0} in {1}ms", file, sw.ElapsedMilliseconds);

            return o;
        }

        public static FHXObject FromXMLObject(XmlNode node)
        {
            FHXObject o = new FHXObject();

            foreach(XmlAttribute a in node.Attributes)
            {
                if(a.Name == "TYPE")
                {
                    o.Type = a.Value;
                }
                else
                {
                    o.AddParameter(new FHXParameter(a.Name, a.Value, true));
                }
            }

            foreach(XmlNode c in node.ChildNodes)
            {
                if(c.Name == "parameters")
                {
                    foreach (XmlAttribute a in c.Attributes)
                    {
                        o.AddParameter(new FHXParameter(a.Name, a.Value, false));
                    }
                }
                else if(c.Name == "children")
                {
                    foreach(XmlNode cc in c.ChildNodes)
                    {
                        o.AddChild(FromXMLObject(cc));
                    }
                }
                else
                {
                    Console.WriteLine("Unknown tag : {0}", c.Name);
                }
            }

            return o;
        }

        public static void ToXML(FHXObject root, string file)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode rootNode = xmlDoc.CreateElement("root");
            xmlDoc.AppendChild(rootNode);

            ToXML(root, rootNode, xmlDoc);

            xmlDoc.Save(file);
        }

        public static void ToXML(FHXObject o, XmlNode parentNode, XmlDocument doc)
        {
            XmlNode oNode = doc.CreateElement("object");
            parentNode.AppendChild(oNode);

            XmlAttribute attr = doc.CreateAttribute("TYPE");
            attr.Value = o.Type;
            oNode.Attributes.Append(attr);

            if (o.Parameters.Count > 0)
            {
                foreach (FHXParameter p in o.Parameters.Where((i) => i.Mandatory))
                {
                    XmlAttribute a = doc.CreateAttribute(p.Name);
                    a.Value = p.Value;
                    oNode.Attributes.Append(a);
                }

                if(o.Parameters.Where((i) => i.Mandatory == false).ToList().Count > 0)
                {
                    XmlNode cNode = doc.CreateElement("parameters");
                    oNode.AppendChild(cNode);

                    foreach (FHXParameter p in o.Parameters.Where((i) => i.Mandatory == false))
                    {
                        XmlAttribute a = doc.CreateAttribute(p.Name);
                        a.Value = p.Value;
                        cNode.Attributes.Append(a);
                    }
                }
            }

            if(o.Children.Count > 0)
            {
                XmlNode cNode = doc.CreateElement("children");
                oNode.AppendChild(cNode);
                foreach (FHXObject c in o.Children)
                {
                    ToXML(c, cNode, doc);
                }
            }
        }
    }
}
