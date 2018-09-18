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
            List<FHXObject> objects = new List<FHXObject>();
            List<FHXParameter> parameters = new List<FHXParameter>();


            FHXObject root = new FHXObject("ROOT", "Project");
            objects.Add(root);

            bool waitingForBody = false; //Flag to see if we started an item and are looking for a bracketed body

            FHXObject currentObject = root;

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
                else if (t.Type == TokenType.WHITESPACE)
                {
                    //Skips whitespaces
                    Next();
                }
                else if(t.Type == TokenType.OPEN_BRACKET)
                {
                    if (waitingForBody)
                    {
                        waitingForBody = false;
                    }
                    else
                    {
                        Console.WriteLine("Brackets without an item identifier");
                    }
                    Next();
                }
                else if (t.Type == TokenType.CLOSE_BRACKET)
                {
                    currentObject = currentObject.Parent;
                    Next();
                }
                else if (t.Type == TokenType.TEXT)
                {
                    string s = Next().Value;

                    if(Peek().Type == TokenType.WHITESPACE)
                    {
                        //New object
                        FHXObject o = NewObject(s);
                        currentObject.AddChild(o);
                        currentObject = o;
                        objects.Add(o);
                        waitingForBody = true;
                    }
                    else if (Peek().Type == TokenType.EQUAL)
                    {
                        Next(); //Skips the equal sign

                        if (Peek().Type == TokenType.WHITESPACE)
                        {
                            Next();
                            FHXObject o = new FHXObject();
                            o.Name = s;
                            currentObject.AddChild(o);
                            currentObject = o;
                            objects.Add(o);
                            waitingForBody = true;
                        }
                        else
                        {
                            string value = Next().Value; //Retieves the parameter's value
                            FHXParameter p = new FHXParameter(s, value);
                            currentObject.AddParameter(p);
                            parameters.Add(p);
                        }

                       
                    }
                    else
                    {
                        //Unexpected token
                        Console.WriteLine("Unexpected token while parsing TEXT : {0}", Peek().GetTypeString());
                    }
                }
                else
                {
                    Console.WriteLine("Unhandled token : {0}", Peek().GetTypeString());
                    Next();
                }
            }

            return root;
        }

        FHXObject NewObject(string type)
        {
            FHXObject o = new FHXObject();
            o.Type = type;
            return o;
        }
    }
}
