using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.AST
{
    public class GetVariableValueExpression : ExpressionNode
    {
        private object name;

        public GetVariableValueExpression(object name)
        {
            this.name = name;
        }

        public object Name => name;
    }
}
