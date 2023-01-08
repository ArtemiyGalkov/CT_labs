using CT.Lab3.AST;
using CT.Lab3.BASIC;
using CT.Lab3.BASIC.SpecificASTNodes;
using CT.Lab3.CommonCode.ConcreteLanguageSyntax;
using NUnit.Framework;
using System.IO;
using System.Text.RegularExpressions;

namespace CT.Lab3.BASICTests
{
    public class Tests
    {
        private LexemParser lexemParser;
        private BASICParser astParser;

        [SetUp]
        public void Setup()
        {
            lexemParser = new LexemParser(new BASICSyntaxProvider());
            astParser = new BASICParser();
        }

        [Test]
        public void ProgramWithConditionTest()
        {
            var code = File.ReadAllText("Code.txt");
            code = Regex.Replace(code, @"\t|\n|\r", " ");

            var lexems = lexemParser.ParseToLexems(code);
            var tree = astParser.ParseLexemas(lexems.ToArray());
            Assert.AreEqual(12, tree.Count);

            var conditionStatement = tree[9] as ConditionNode;
            Assert.NotNull(conditionStatement);

            var condition = conditionStatement.Condition as BinaryExpressionNode;
            var thenNode = conditionStatement.ThenNode as GoToNode;
            var elseNode = conditionStatement.ElseNode as PrintNode;

            Assert.NotNull(condition);
            Assert.AreEqual("=", condition.Operation.ToString());
            Assert.AreEqual("A$", (condition.LeftNode as Variable).Name);
            Assert.AreEqual("\"Y\"", (condition.RightNode as LiteralExpression).Value.ToString());

            Assert.NotNull(thenNode);
            Assert.AreEqual(30, thenNode.GoToLine);

            Assert.NotNull(elseNode);
            Assert.AreEqual("\"Else block\"", elseNode.PrintMessage);

        }
    }
}