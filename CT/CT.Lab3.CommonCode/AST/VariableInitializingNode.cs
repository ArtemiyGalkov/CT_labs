using CT.Lab3.AST;
using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.CommonCode.AST
{
    public class VariableInitializingNode : VariableDeclarationNode
    {
        private ExpressionNode assigningExpression;

        public VariableInitializingNode(string[] variablesName, string type, ExpressionNode assigningExpression) : base (variablesName, type)
        {
            this.assigningExpression = assigningExpression;
        }

        public ExpressionNode ValueExpression => assigningExpression;
    }
}
