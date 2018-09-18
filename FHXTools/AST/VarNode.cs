using System;
using System.Collections.Generic;
using System.Text;

namespace FHXTools.AST
{
    class VarNode : ASTNode
    {
        public string Name { get; }
        public bool Local { get; }
        public VarNode(string name, bool local = false) : base("var")
        {
            this.Name = name;
            this.Local = local;
        }

        override public string GetValue()
        {
            return "";
        }
    }
}
