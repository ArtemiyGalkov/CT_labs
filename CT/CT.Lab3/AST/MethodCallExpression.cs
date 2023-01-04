using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.AST
{
    public class MethodCallExpression : ExpressionNode
    {
        private string methodName;
        private ExpressionNode[] arguments;

        public MethodCallExpression(string methodName, ExpressionNode[] arduments)
        {
            this.methodName = methodName;
            this.arguments = arduments;
        }

        public string MethodName
        {
            get => methodName;
        }

        public ExpressionNode[] Arguments
        {
            get => arguments;
        }
    }
}
