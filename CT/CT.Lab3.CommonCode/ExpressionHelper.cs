using CT.Lab3.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CT.Lab3.CommonCode
{
    public class ExpressionHelper
    {
        private ISyntaxProvider syntaxProvider;

        public ExpressionHelper(ISyntaxProvider syntaxProvider)
        {
            this.syntaxProvider = syntaxProvider;
        }

        public ExpressionNode ParseToExpressionTree(List<object> operands)
        {
            var postfixList = CreatePostfixList(operands);
            return CountExpression(postfixList);
        }

        private List<object> CreatePostfixList(List<object> operands)
        {
            Stack<string> operandsStack = new Stack<string>();
            List<object> postfixList = new List<object>();

            while (operands.Count > 0)
            {
                var currentOperand = operands.First();
                operands.RemoveAt(0);

                if (currentOperand is ExpressionNode)
                {
                    postfixList.Add(currentOperand);
                }
                else if (currentOperand is Lexem lexem)
                {
                    if (lexem.Code.ToString() == "(")
                    {
                        operandsStack.Push("(");
                    }
                    else if (lexem.Code.ToString() == ")")
                    {
                        while (operandsStack.Peek() != "(")
                        {
                            postfixList.Add(operandsStack.Pop());
                        }

                        operandsStack.Pop();
                    }
                    else if (lexem.Type is LexemType.Operator)
                    {
                        string currentOperator = lexem.Code.ToString();
                        int currentPriority = GetOperatorPriority(currentOperator);

                        while (operandsStack.Count > 0 && GetOperatorPriority(operandsStack.Peek()) >= currentPriority)
                        {
                            postfixList.Add(operandsStack.Pop());
                        }

                        operandsStack.Push(currentOperator);
                    }
                }
            }

            while (operandsStack.Count > 0)
            {
                postfixList.Add(operandsStack.Pop());
            }

            return postfixList;
        }

        private ExpressionNode CountExpression(List<object> postfixList)
        {
            Stack<ExpressionNode> expressions = new Stack<ExpressionNode>();

            foreach (var operand in postfixList)
            {
                if (operand is ExpressionNode expression)
                {
                    expressions.Push(expression);
                }
                else if (operand is string operation)
                {
                    var right = expressions.Pop();
                    var left = expressions.Pop();

                    var newExpression = new BinaryExpressionNode(left, right, operation);
                    expressions.Push(newExpression);
                }
            }

            return expressions.Pop();
        }

        private int GetOperatorPriority(string operation)
        {
            if (operation == ")")
            {
                return int.MaxValue;
            }

            if (operation == "(")
            {
                return int.MinValue;
            }

            return syntaxProvider.OperatorsPriority[operation];
        }
    }
}
