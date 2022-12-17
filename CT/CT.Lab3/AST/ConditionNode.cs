using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.AST
{
    public class ConditionNode : ASTNode
    {
        private ExpressionNode condition;
        private ASTNode thenNode;
        private ASTNode elseNode;

        public ConditionNode(ExpressionNode condition, ASTNode thenNode, ASTNode elseNode)
        {
            this.condition = condition;
            this.thenNode = thenNode;
            this.elseNode = elseNode;
        }

        public override object Value => throw new NotImplementedException();
    }
}
