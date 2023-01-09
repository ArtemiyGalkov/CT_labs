using CT.Lab3.AST;
using CT.Lab3.Modula;
using NUnit.Framework;
using System.IO;
using System.Text.RegularExpressions;

namespace CT.Lab3.ModulaTests
{
    public class Tests
    {
        private LexemParser lexemParser;
        private ModulaParser astParser;

        [SetUp]
        public void Setup()
        {
            lexemParser = new LexemParser(new ModulaSyntaxProvider());
            astParser = new ModulaParser();
        }

        [Test]
        public void ProgramWithConditionTest()
        {
            var code = File.ReadAllText("Code.txt");
            code = Regex.Replace(code, @"\t|\n|\r", " ");

            var lexems = lexemParser.ParseToLexems(code);
            var tree = astParser.ParseLexemas(lexems.ToArray());

            Assert.AreEqual(3, tree.Count);
            Assert.True(tree[0] is ModuleDeclarationNode);
            Assert.AreEqual("IfDemo", (tree[0] as ModuleDeclarationNode).Name);

            Assert.True(tree[1] is VariablesSectionNode);
            var variablesDeclarations = (tree[1] as VariablesSectionNode).VariableDeclarationNodes;

            Assert.AreEqual(1, variablesDeclarations.Length);
            Assert.AreEqual(new string[] { "Index1" }, variablesDeclarations[0].VariablesNames);
            Assert.AreEqual("INTEGER", variablesDeclarations[0].Type);

            Assert.True(tree[2] is ProgramBlockNode);
            var statements = (tree[2] as ProgramBlockNode).Nodes;
            Assert.AreEqual(5, statements.Length);
            Assert.True(statements[0] is ConditionNode);
            Assert.True(statements[1] is ConditionNode);
            Assert.True(statements[2] is MethodCallExpression);
            Assert.True(statements[3] is ConditionNode);
            Assert.True(statements[4] is MethodCallExpression);

            Assert.AreEqual("WriteLn", (statements[2] as MethodCallExpression).MethodName);

            var condition = statements[3] as ConditionNode;
            Assert.True(condition.Condition is BinaryExpressionNode);

            var conditionExpression = condition.Condition as BinaryExpressionNode;

            Assert.AreEqual("=", conditionExpression.Operation.ToString());
            Assert.True(conditionExpression.LeftNode is Variable);
            Assert.True(conditionExpression.RightNode is LiteralExpression);
            Assert.AreEqual("Index1", (conditionExpression.LeftNode as Variable).Name);
            Assert.AreEqual("2", (conditionExpression.RightNode as LiteralExpression).Value.ToString());

            Assert.True(condition.ElseNode is ConditionNode);
            Assert.True(condition.ThenNode is ProgramBlockNode);
            Assert.AreEqual(1, (condition.ThenNode as ProgramBlockNode).Nodes.Length);

            var methodCall = (condition.ThenNode as ProgramBlockNode).Nodes[0] as MethodCallExpression;
            Assert.NotNull(methodCall);
            Assert.AreEqual("WriteString", methodCall.MethodName);
            Assert.True(methodCall.Arguments[0] is LiteralExpression);
            Assert.AreEqual("\"Index1 is 2\"", (methodCall.Arguments[0] as LiteralExpression).Value.ToString());
        }
    }
}