using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.AST
{
    public class MethodCallExpression : ExpressionNode
    {
        private string methodName;
        private ExpressionNode[] ardumentsNodes;

        public MethodCallExpression(string methodName, ExpressionNode[] ardumentsNodes)
        {
            this.methodName = methodName;
            this.ardumentsNodes = ardumentsNodes;
        }

        public override object Value => throw new NotImplementedException();

        public string MethodName()
        {
            return methodName
        }

        public ASTNode[] Arguments()
        {
            return ardumentsNodes;
        }
    }
}
