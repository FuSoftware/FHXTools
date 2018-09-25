using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHXTools.Parsing
{
    class TokenListParser : Parser
    {
        List<Token> mTokens;
        Token current;
        int i;

        public TokenListParser(List<Token> tokens)
        {
            this.mTokens = tokens;
            i = 0;
        }

        protected override Token ReadNextToken()
        {
            return mTokens[i++];
        }

        protected override Token Peek()
        {
            if (current == null)
            {
                current = this.ReadNextToken();
            }

            return current;
        }

        protected override Token Next()
        {
            Token t = current;
            current = null;

            if (t == null)
            {
                t = this.ReadNextToken();
            }
            return t;
        }

        protected override bool EOF()
        {
            return i >= mTokens.Count;
        }
    }
}
