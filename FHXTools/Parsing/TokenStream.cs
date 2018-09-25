using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace FHXTools.Parsing
{
    class TokenStream
    {
        public InputStream Input { get; }
        Token current;

        public TokenStream(string input)
        {
            this.Input = new InputStream(input);
        }

        public TokenStream(InputStream input)
        {
            this.Input = input;
        }

        delegate bool Predicate(char c);

        bool IsLetter(char c)
        {
            return char.IsLetter(c);
        }

        bool IsWhitespace(char c)
        {
            return char.IsWhiteSpace(c);
        }

        bool IsNumeric(char c)
        {
            return char.IsNumber(c);
        }

        bool IsText(char c)
        {
            return (char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsSymbol(c)) && !("@=/*{}".Contains(c));
        }

        bool IsOpenBracket(char c)
        {
            return @"{".Contains(c);
        }

        bool IsCloseBracket(char c)
        {
            return @"}".Contains(c);
        }

        bool IsEqual(char c)
        {
            return @"=".Contains(c);
        }

        bool IsStar(char c)
        {
            return @"*".Contains(c);
        }

        bool IsSlash(char c)
        {
            return @"/".Contains(c);
        }

        bool IsQuote(char c)
        {
            return c == '"';
        }

        bool IsDoubleQuote()
        {
            return IsQuote(Input.Peek()) && (IsQuote(Input.PeekPrevious()) || IsQuote(Input.PeekNext()));
        }

        bool IsCommentChar(char c)
        {
            return @"/*".Contains(c);
        }

        string ReadWhile(Predicate predicate)
        {
            string str = "";
            while (!Input.EOF() && predicate(Input.Peek()))
                str += Input.Next();
            return str;
        }

        string ReadUntil(Predicate predicate)
        {
            string str = "";
            while (!Input.EOF() && !predicate(Input.Peek()))
                str += Input.Next();
            return str;
        }

        private Token ReadNext()
        {
            if (Input.EOF()) return null;
            char ch = Input.Peek();

            if (IsQuote(ch))
            {
                Input.Next(); //Skip first quote

                string str = "";
                while (!Input.EOF())
                {
                    char c = Input.Next();
                    if (IsQuote(c))
                    {
                        c = Input.Peek();
                        if (IsQuote(c))
                        {
                            str += "\"";
                            Input.Next();
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        str += c;
                    }
                }

                return new Token(TokenType.STRING, str);
            }
            else if(IsOpenBracket(ch))
            {
                return new Token(TokenType.OPEN_BRACKET, Input.Next());
            }
            else if (IsCloseBracket(ch))
            {
                return new Token(TokenType.CLOSE_BRACKET, Input.Next());
            }
            else if (IsEqual(ch))
            {
                return new Token(TokenType.EQUAL, Input.Next());
            }
            else if (IsWhitespace(ch))
            {
                return new Token(TokenType.WHITESPACE, ReadWhile(IsWhitespace));
            }
            else if (IsCommentChar(ch))
            {
                char c = Input.Next();
                char d = Input.Peek();

                if(c == '/' && d == '*')
                {
                    Input.Next();
                    return new Token(TokenType.COMMENT_START, "/*");
                }
                else if (c == '*' && d == '/')
                {
                    Input.Next();
                    return new Token(TokenType.COMMENT_END, "*/");
                }
                else
                {
                    return new Token(TokenType.TEXT, c);
                }
            }
            else if (IsText(ch))
            {
                return new Token(TokenType.TEXT, ReadWhile(IsText));
            }
            else
            {
                this.Input.Croak("Can't handle character: " + ch);
                return null;
            }
        }

        public Token Peek()
        {
            if(current == null)
            { 
                current = this.ReadNext();
            }

            return current;
        }

        public Token Next()
        {
            Token t = current;
            current = null;

            if(t == null)
            {
                t = this.ReadNext();
            }
            return t;
        }

        public bool EOF()
        {
            return this.Peek() == null;
        }
    }
}
