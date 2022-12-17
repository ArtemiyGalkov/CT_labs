using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.AST
{
    public class LiteralExpression : UnaryExpression
    {
        public LiteralExpression(object value) : base(UnaryExpressionType.Literal, value)
        {
        }
    }
}
