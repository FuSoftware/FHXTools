using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHXTools.FHX
{
    class FHXHierarchyBuilder
    {
        public static void BuildDeltaVHierarchy(FHXObject obj)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            FHXObject root = obj.GetRoot();
            List<FHXObject> AllChildren = root.GetAllChildren();


            //Replace FBs with DEFINITION by their definition
            List<FHXObject> FUNCTION_BLOCK = AllChildren.Where(i => i.Type == "FUNCTION_BLOCK" && i.Parameters.Any(j => j.Name == "DEFINITION")).ToList();
            List<FHXObject> FUNCTION_BLOCK_DEFINITION = AllChildren.Where(i => i.Type == "FUNCTION_BLOCK_DEFINITION").ToList();
            foreach (FHXObject fb in FUNCTION_BLOCK)
            {
                FHXObject parent = fb.Parent;
                fb.Parent.RemoveChild(fb);
                fb.SetParent(null);

                if (FUNCTION_BLOCK_DEFINITION.Any(i => i.GetName() == fb.GetParameter("DEFINITION").Value))
                {
                    foreach (FHXObject newChild in FUNCTION_BLOCK_DEFINITION.Where(i => i.GetName() == fb.GetParameter("DEFINITION").Value))
                    {
                        newChild.Name = fb.GetName();
                        newChild.Type = "FUNCTION_BLOCK";
                        newChild.Parent.RemoveChild(newChild);
                        newChild.SetParent(null);
                        parent.AddChild(newChild);
                    }
                }
            }
            sw.Stop();
            Console.WriteLine("{0} took {1}ms", "Loading classes", sw.ElapsedMilliseconds);

            //Removes the VALUE Objects and sets their parameters to their parent.
            sw.Restart();
            List<FHXObject> VALUE = AllChildren.Where(i => i.Type == "VALUE").ToList();
            foreach (FHXObject fb in VALUE)
            {
                FHXObject parent = fb.Parent;

                foreach (FHXParameter p in fb.Parameters)
                {
                    parent.AddParameter(p);
                    fb.SetParent(null);
                }
            }
            sw.Stop();
            Console.WriteLine("{0} took {1}ms", "Removing VALUEs", sw.ElapsedMilliseconds);


            //Replace ATTRIBUTE by ATTRIBUTE_INSTANCE when needed
            sw.Restart();
            List<FHXObject> ATTRIBUTE_INSTANCE = AllChildren.Where(i => i.Type == "ATTRIBUTE_INSTANCE").ToList();
            foreach (FHXObject attr in ATTRIBUTE_INSTANCE)
            {
                FHXObject parent = attr.Parent;

                List<FHXObject> ATTRIBUTE = parent.GetAllChildren().Where(i => i.Type == "ATTRIBUTE" && i.GetName() == attr.GetName()).ToList();

                if (ATTRIBUTE.Count > 1)
                {
                    Console.WriteLine("Multiple instances of {0} found", attr.GetName());
                }

                foreach (var a in ATTRIBUTE)
                {
                    if (a.Parent != null)
                    {
                        //Adds the ATTRIBUTE_INSTANCE in the parent of the ATTRIBUTE
                        attr.SetParent(null);
                        a.Parent.AddChild(attr);

                        //Removes the ATTRIBUTE from its parent
                        a.Parent.RemoveChild(a);
                        a.SetParent(null);
                    }
                }
            }
            sw.Stop();
            Console.WriteLine("{0} took {1}ms", "Replacing ATTRIBUTEs", sw.ElapsedMilliseconds);

            //Removes the useless items by type
            sw.Restart();
            List<string> unused_types = new List<string>() { "WIRE", "GRAPHICS", "FUNCTION_BLOCK_TEMPLATE", "FUNCTION_BLOCK_DEFINITION" };
            List<string> unused_names = new List<string>() { "RECTANGLE", "POSITION", "ORIGIN", "END" };
            List<FHXObject> uc = AllChildren.Where(i => unused_types.Contains(i.Type) || unused_names.Contains(i.Name)).ToList();
            foreach (FHXObject fb in uc)
            {
                fb.Parent.RemoveChild(fb);
                fb.SetParent(null);
            }
            sw.Stop();
            Console.WriteLine("{0} took {1}ms", "Clearing hierarchy", sw.ElapsedMilliseconds);
        }
    }
}
