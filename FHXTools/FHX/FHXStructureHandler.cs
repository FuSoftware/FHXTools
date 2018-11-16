using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHXTools.FHX
{
    public class FHXStructureHandler
    {
        public class FHXNode{
            public FHXNode(string label, FHXObject data = null, List<FHXNode> children = null)
            {
                this.Label = label;
                this.Data = data;
                this.Children = children==null?new List<FHXNode>():children;
            }

            public string Label { get; set; }
            public FHXObject Data { get; set; }
            public List<FHXNode> Children { get; set; }
        }

        FHXObject Root;

        public FHXStructureHandler(FHXObject root)
        {
            this.Root = root;
        }

        public List<FHXNode> Areas {
            get
            {
                IEnumerable<FHXParameter> lp = Root.GetAllParameters().Where((p) => p.Name == "PLANT_AREA").GroupBy(p=>p.Name).Select(p=>p.First());
                List<FHXNode> nodes = new List<FHXNode>();
                foreach (var p in lp)
                {
                    nodes.Add(new FHXNode(p.Value));
                }
                return nodes;
            }
        }

        public List<FHXNode> ModulesArea(string area)
        {
            IEnumerable<FHXObject> lp = Root.GetAllChildren().Where((o) => o.Type == "MODULE" && o.GetParameterValue("PLANT_AREA")==area).Distinct();
            List<FHXNode> nodes = new List<FHXNode>();

            foreach (FHXObject obj in lp)
            {
                nodes.Add(new FHXNode(obj.Name, obj));
            }

            return nodes;
        }
    }
}
