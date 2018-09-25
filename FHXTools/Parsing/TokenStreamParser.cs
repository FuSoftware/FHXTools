using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHXTools.Parsing
{
    class TokenStreamParser : Parser
    {
        public TokenStream mTokenStream { get; }
        Token current;

        public TokenStreamParser(TokenStream token_stream)
        {
            this.mTokenStream = token_stream;
        }

        protected override Token ReadNextToken()
        {
            return mTokenStream.Next();
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
            return mTokenStream.EOF();
        }
    }
}
