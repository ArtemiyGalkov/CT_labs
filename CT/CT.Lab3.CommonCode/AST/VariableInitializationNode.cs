using CT.Lab3.AST;
using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.CommonCode.AST
{
    public class VariableInitializationNode : VariableDeclarationNode
    {
        private ExpressionNode[] assigningExpression;

        public VariableInitializationNode(string[] variablesName, string type, ExpressionNode[] assigningExpression) : base (variablesName, type)
        {
            this.assigningExpression = assigningExpression;
        }

        public ExpressionNode[] ValueExpression => assigningExpression;
    }
}
