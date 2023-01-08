using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.CommonCode
{
    public interface ISyntaxProvider
    {
        public char[] Punctuations { get; }
        public char[] Operators { get; }
        public char[] Digits { get; }
        public char[] StringContainer { get; }
        public char[] CharContainer { get; }
        public string[] KeyWords { get; }
    }
}
