using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3
{
    public class Lexem
    {
        public Lexem(string code)
        {
            Code = new StringBuilder(code);
        }

        public StringBuilder Code { get; private set; } = new StringBuilder();

        public LexemType Type { get; set; }
    }

    public enum LexemType
    {
        Punctuation,
        Number,
        String,
        Operator,
        Keyword,
        Identifier
    }
}
