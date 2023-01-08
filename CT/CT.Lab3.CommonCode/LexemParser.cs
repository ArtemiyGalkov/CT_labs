using CT.Lab3.CommonCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CT.Lab3
{
    public class LexemParser
    {
        private string _code;
        private List<Lexem> lexems;
        private ISyntaxProvider syntaxProvider;

        public LexemParser(ISyntaxProvider syntaxProvider)
        {
            this.syntaxProvider = syntaxProvider;
        }

        public List<Lexem> ParseToLexems(string code)
        {
            this._code = code;
            lexems = new List<Lexem>();

            for (int i = 0; i < _code.Length; i++)
            {
                char curChar = _code[i];
                if (char.IsWhiteSpace(curChar))
                {
                    continue;
                }
                else if (syntaxProvider.Punctuations.Contains(curChar))
                {
                    lexems.Add(new Lexem(curChar.ToString()) { Type = LexemType.Punctuation });
                }
                else if (syntaxProvider.Operators.Contains(curChar))
                {
                    lexems.Add(new Lexem(curChar.ToString()) { Type = LexemType.Operator });
                }
                else if (syntaxProvider.Digits.Contains(curChar))
                {
                    lexems.Add(ReadNumber(ref i));
                }
                else if (syntaxProvider.StringContainer.Contains(curChar))
                {
                    lexems.Add(ReadString(ref i));
                }
                else if (syntaxProvider.CharContainer.Contains(curChar))
                {
                    lexems.Add(ReadChar(ref i));
                }
                else
                {
                    var lexem = ReadLetters(ref i);

                    if (syntaxProvider.KeyWords.Contains(lexem.Code.ToString()))
                    {
                        lexem.Type = LexemType.Keyword;
                    }
                    else
                    {
                        lexem.Type = LexemType.Identifier;
                    }

                    lexems.Add(lexem);
                }
            }

            return lexems;
        }

        private Lexem ReadNumber(ref int i)
        {
            var lexem = new Lexem(_code[i].ToString()) { Type = LexemType.Number };

            while (i < _code.Length && syntaxProvider.Digits.Contains(_code[i + 1]))
            {
                lexem.Code.Append(_code[i + 1]);
                i++;
            }

            return lexem;
        }

        private Lexem ReadString(ref int i)
        {
            var lexem = new Lexem(_code[i].ToString()) { Type = LexemType.String };

            do
            {
                lexem.Code.Append(_code[i + 1]);
                i++;
            }
            while (i < _code.Length && !syntaxProvider.StringContainer.Contains(_code[i]));

            return lexem;
        }

        private Lexem ReadChar(ref int i)
        {
            var lexem = new Lexem(_code[i].ToString()) { Type = LexemType.String };

            if (i + 2 >= _code.Length || syntaxProvider.CharContainer.Contains(_code[i + 2]))
            {
                throw new Exception();
            }

            lexem.Code.Append(_code[i + 1]);
            lexem.Code.Append(_code[i + 2]);

            i += 2;

            return lexem;
        }

        private Lexem ReadLetters(ref int i)
        {
            var lexem = new Lexem(_code[i].ToString());

            while (i < _code.Length - 1 && (Char.IsLetter(_code[i + 1]) || Char.IsDigit(_code[i + 1])))
            {
                lexem.Code.Append(_code[i + 1]);
                i++;
            }

            return lexem;
        }
    }
}
