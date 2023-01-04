using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.AST
{
    public class BinaryExpressionNode : ExpressionNode
    {
        private ASTNode leftNode;
        private ASTNode rightNode;
        private object operation;

        public BinaryExpressionNode(ASTNode leftNode, ASTNode rightNode, object operation)
        {
            this.leftNode = leftNode;
            this.rightNode = rightNode;
            this.operation = operation;
        }

        public ASTNode LeftNode { get => leftNode; }
        public ASTNode RightNode { get => rightNode; }
        public object Operation { get => operation; }
    }
}
