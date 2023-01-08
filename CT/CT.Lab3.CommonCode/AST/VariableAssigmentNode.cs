using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.AST
{
    public class VariableAssigmentNode : BinaryExpressionNode
    {
        public VariableAssigmentNode(IdentifierNode identifier, ExpressionNode value) : base(identifier, value, ":=")
        {
        }
    }
}
