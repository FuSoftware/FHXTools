using System;
using System.Collections.Generic;
using System.Text;

namespace FHXTools.AST
{
    class ASTNode
    {
        public string Type { get; }
        public List<ASTNode> Children = new List<ASTNode>();

        public ASTNode()
        {
            this.Type = "node";
        }

        public ASTNode(string type)
        {
            this.Type = type;
        }

        public void AddChild(ASTNode child)
        {
            this.Children.Add(child);
        }

        virtual public string GetValue()
        {
            return "";
        }
    }
}