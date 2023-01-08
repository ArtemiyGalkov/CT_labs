﻿using CT.Lab3.CommonCode.ConcreteLanguageSyntax;
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
                "break","default","func","interface","struct","select","case","Go","map","else","package",
                "Goto","switch","const","if","range","type","continue","for","import","return","var",
            };
    }
}
