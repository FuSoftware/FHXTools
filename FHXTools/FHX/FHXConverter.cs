using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FHXTools.FHX
{
    class FHXConverter
    {
        public static FHXObject FromXML(string file)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(file);

            XmlNode root = xmlDoc.FirstChild;

            return FromXMLObject(root.FirstChild, xmlDoc);

            
        }

        public static FHXObject FromXMLObject(XmlNode node, XmlDocument doc)
        {

        }

        public static FHXParameter FromXMLParameter(XmlNode node, XmlDocument doc)
        {

        }

        public static void ToXML(FHXObject root, string file)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode rootNode = xmlDoc.CreateElement("root");
            xmlDoc.AppendChild(rootNode);

            ToXML(root, rootNode, xmlDoc);

            xmlDoc.Save(file);
        }

        public static void ToXML(FHXParameter p, XmlNode parentNode, XmlDocument doc)
        {
            XmlNode oNode = doc.CreateElement("parameter");
            parentNode.AppendChild(oNode);

            XmlAttribute a = doc.CreateAttribute("name");
            a.Value = p.Name;
            XmlAttribute b = doc.CreateAttribute("value");
            b.Value = p.Value;

            oNode.Attributes.Append(a);
            oNode.Attributes.Append(b);
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

                XmlNode cNode = doc.CreateElement("parameters");
                oNode.AppendChild(cNode);

                foreach (FHXParameter p in o.Parameters.Where((i)=>i.Mandatory == false))
                {
                    ToXML(p, cNode, doc);
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
