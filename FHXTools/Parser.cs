using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHXTools
{
    class Parser
    {
        List<Token> mTokens;
        
        public Parser(List<Token> tokens)
        {
            this.mTokens = tokens;
        }
    }
}
