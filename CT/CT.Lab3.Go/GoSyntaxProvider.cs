using CT.Lab3.CommonCode.ConcreteLanguageSyntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.Modula
{
    public class GoSyntaxProvider : CommonLanguageProvider
    {
        public override char[] Punctuations => new char[] { '{', '}', '(', ')', ',', ';', '.', };

        public override char[] StringContainer => new char[] { '"', };

        public override char[] CharContainer => new char[] { '\'', };

        public override string[] KeyWords => new string[]
            { 
                "break","default","func","interface","select","case","Go","map","Struct","else",
                "Goto","Switch","const","if","range","Type","continue","for","import","return","Var" 
            };
    }
}
