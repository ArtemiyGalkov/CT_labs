using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.AST
{
    public class LiteralExpression : ExpressionNode
    {
        private object value;

        public LiteralExpression(object value)
        {
            this.value = value;
        }

        public object Value => value;
    }
}
