using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.AST
{
    public class UnaryExpression : ExpressionNode
    {
        private UnaryExpressionType type;
        private object value;

        public UnaryExpression(UnaryExpressionType type, object value)
        {
            this.type = type;
            this.value = value;
        }

        public override object Value => value;

        public UnaryExpressionType Type => type;
    }

    public enum UnaryExpressionType
    {
        Variable,
        MethodCall,
        Literal,
    }
}
