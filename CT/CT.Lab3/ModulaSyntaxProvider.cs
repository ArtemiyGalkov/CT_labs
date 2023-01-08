using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.CommonCode.ConcreteLanguageSyntax
{
    public class ModulaSyntaxProvider : DefaultLanguageProvider
    {
        public override char[] Punctuations => new char[] { '{', '}', '(', ')', ',', ';', '.', };

        public override char[] StringContainer => new char[] { '"', };

        public override char[] CharContainer => new char[] { '\'', };

        public override string[] KeyWords => new string[]
            { "MODULE", "CONST", "VAR", "BEGIN", "END", "REPEAT", "UNTIL", "WHILE", "FOR",
            "TO", "BY", "DO", "LOOP", "IF", "THEN", "ELSE", "ELSIF", "EXIT", "CASE", "OF" };
    }
}
