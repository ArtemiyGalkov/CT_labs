using CT.Lab3.AST;
using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.CommonCode.AST
{
    public class ReturnNode : ASTNode
    {
        private ExpressionNode expressionToReturn;

        public ReturnNode(ExpressionNode expressionToReturn)
        {
            this.expressionToReturn = expressionToReturn;
        }
    }
}
