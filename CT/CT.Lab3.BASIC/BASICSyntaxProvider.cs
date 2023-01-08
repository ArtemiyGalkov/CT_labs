using CT.Lab3.CommonCode.ConcreteLanguageSyntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.BASIC
{
    public class BASICSyntaxProvider : CommonLanguageProvider
    {
        public override char[] Punctuations => new char[] { '{', '}', '(', ')', ',', ';', '.', };

        public override char[] StringContainer => new char[] { '"', };

        public override char[] CharContainer => new char[] { };

        public override string[] KeyWords => new string[]
            { "LET", "INPUT", "PRINT", "FOR", "NEXT", "GOTO", "IF", "THEN", "ELSE", "OR", "DEF", "READ", "DATA", "END" };
    }
}
