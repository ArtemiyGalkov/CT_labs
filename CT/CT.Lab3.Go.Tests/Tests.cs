using CT.Lab3.AST;
using CT.Lab3.CommonCode;
using CT.Lab3.CommonCode.AST;
using CT.Lab3.CommonCode.ConcreteLanguageSyntax;
using CT.Lab3.Go;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace CT.Lab3.Go.Tests
{
    public class Tests
    {
        private LexemParser lexemParser;
        private GoParser astParser;

        [SetUp]
        public void Setup()
        {
            lexemParser = new LexemParser(new GoSyntaxProvider());
            astParser = new GoParser();
        }

        [Test]
        public void ProgramWithConditionTest()
        {
            var code = File.ReadAllText("Code.txt");
            code = Regex.Replace(code, @"\t|\n|\r", " ");

            var lexems = lexemParser.ParseToLexems(code);
            var tree = astParser.ParseLexemas(lexems.ToArray());
            Assert.AreEqual(6, tree.Count);

            var methodDeclaration = tree[5] as MethodDeclarationNode;
            Assert.NotNull(methodDeclaration);

            Assert.AreEqual("max", methodDeclaration.MethodName);
            Assert.AreEqual(2, methodDeclaration.Parameters.Length);
            Assert.AreEqual("int", methodDeclaration.ReturnType);
            Assert.AreEqual(4, methodDeclaration.MethodBody.Nodes.Length);

            var condition = methodDeclaration.MethodBody.Nodes[1] as ConditionNode;
            Assert.NotNull(condition);

            Assert.True(condition.Condition is BinaryExpressionNode);

            var conditionExpression = condition.Condition as BinaryExpressionNode;

            Assert.AreEqual(">", conditionExpression.Operation.ToString());
            Assert.True(conditionExpression.LeftNode is Variable);
            Assert.AreEqual("num1", (conditionExpression.LeftNode as Variable).Name);

            Assert.True(condition.ElseNode is ConditionNode);
            Assert.True(condition.ThenNode is ProgramBlockNode);
            Assert.AreEqual(1, (condition.ThenNode as ProgramBlockNode).Nodes.Length);

            var literal = ((condition.ElseNode as ConditionNode).Condition as BinaryExpressionNode).RightNode as LiteralExpression;
            Assert.NotNull(literal);            
            Assert.AreEqual("0", literal.Value.ToString());

            var methodCall = methodDeclaration.MethodBody.Nodes[2] as MethodCallExpression;
            Assert.NotNull(methodCall);
            Assert.AreEqual("fmt.Printf", methodCall.MethodName);
            Assert.True(methodCall.Arguments[0] is LiteralExpression);
            Assert.True(methodCall.Arguments[1] is Variable);
            Assert.AreEqual("\"Max value is : %d\\n\"", (methodCall.Arguments[0] as LiteralExpression).Value.ToString());
        }

        [Test]
        public void ProgramWithConditionTest_arithmetic()
        {
            var code = "func main() { var a int = 1 - 2 / 3 } ";
            code = Regex.Replace(code, @"\t|\n|\r", " ");

            var lexems = lexemParser.ParseToLexems(code);
            var tree = astParser.ParseLexemas(lexems.ToArray());
            var expression = ((tree[0] as MethodDeclarationNode).MethodBody.Nodes[0] as VariableInitializationNode).ValueExpressions[0] as BinaryExpressionNode;

            Assert.AreEqual("-", expression.Operation.ToString());
            Assert.AreEqual("1", (expression.LeftNode as LiteralExpression).Value.ToString());
            Assert.AreEqual("/", (expression.RightNode as BinaryExpressionNode).Operation.ToString());
            Assert.AreEqual("2", ((expression.RightNode as BinaryExpressionNode).LeftNode as LiteralExpression).Value.ToString());
            Assert.AreEqual("3", ((expression.RightNode as BinaryExpressionNode).RightNode as LiteralExpression).Value.ToString());

        }

        [Test]
        public void ProgramWithConditionTest_brackets()
        {
            var code = "func main() { var a int = (1 + 2) * (3 - 4) / 5 } ";
            code = Regex.Replace(code, @"\t|\n|\r", " ");

            var lexems = lexemParser.ParseToLexems(code);
            var tree = astParser.ParseLexemas(lexems.ToArray());
            var expression = ((tree[0] as MethodDeclarationNode).MethodBody.Nodes[0] as VariableInitializationNode).ValueExpressions[0] as BinaryExpressionNode;

            Assert.AreEqual("/", expression.Operation.ToString());
        }
    }
}