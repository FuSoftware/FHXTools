using System;
using System.Collections.Generic;
using System.Text;

namespace FHXTools.AST
{
    class StrNode : ASTNode
    {
        string value = "";

        public StrNode(string value) : base("str")
        {
            this.value = value;
        }

        override public string GetValue()
        {
            return this.value;
        }
    }
}
