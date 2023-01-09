using CT.Lab3.CommonCode.ConcreteLanguageSyntax;
using CT.Lab3.Modula;
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

            var lexemParser = new LexemParser(new ModulaSyntaxProvider());
            var lexems = lexemParser.ParseToLexems(code);

            var astParser = new ModulaParser();
            var tree = astParser.ParseLexemas(lexems.ToArray());

            Console.WriteLine("Done");
        }
    }
}
