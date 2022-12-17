using System;
using System.IO;
using System.Text.RegularExpressions;

namespace CT.Lab3
{
    class Program
    {
        static void Main(string[] args)
        {
            var code = File.ReadAllText("Code.txt");
            code = Regex.Replace(code, @"\t|\n|\r", " ");

            var parser = new LexemParser();

            var result = parser.ParseToLexems(code);
        }
    }
}
