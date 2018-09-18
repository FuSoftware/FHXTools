using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FHXTools.FHX;

namespace FHXTools
{
    class Parser
    {
        List<Token> mTokens;
        Token current;
        int i;
        
        public Parser(List<Token> tokens)
        {
            this.mTokens = tokens;
            i = 0;
        }

        Token ReadNextToken()
        {
            return mTokens[i++];
        }

        Token Peek()
        {
            if (current == null)
            {
                current = this.ReadNextToken();
            }

            return current;
        }

        Token Next()
        {
            Token t = current;
            current = null;

            if (t == null)
            {
                t = this.ReadNextToken();
            }
            return t;
        }

        bool EOF()
        {
            return i >= mTokens.Count;
        }

        public FHXObject ParseAll()
        {
            FHXObject root = new FHXObject("ROOT","Project");

            while (!EOF())
            {
                Token t = Peek();

                if(t.Type == TokenType.COMMENT_START)
                {
                    while(t.Type != TokenType.COMMENT_END)
                    {
                        t = Next();
                    }

                    t = Next(); // Skip last comment token
                }
            }

            return root;
        }
    }
}
