using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.CommonCode.ConcreteLanguageSyntax
{
    public abstract class CommonLanguageProvider : ISyntaxProvider
    {
        public char[] Operators => new char[] { '+', '-', '*', '/', ':', '=', '&', '|', '<', '>' };

        public char[] Digits => new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', };

        public abstract char[] Punctuations { get; }

        public abstract char[] StringContainer { get; }

        public abstract char[] CharContainer { get; }

        public abstract string[] KeyWords { get; }

        public abstract Dictionary<string, int> OperatorsPriority { get; }
    }
}
