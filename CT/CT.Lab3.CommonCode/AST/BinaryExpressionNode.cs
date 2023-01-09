using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.AST
{
    public class BinaryExpressionNode : ExpressionNode
    {
        private ExpressionNode leftNode;
        private ExpressionNode rightNode;
        private object operation;

        public BinaryExpressionNode(ExpressionNode leftNode, ExpressionNode rightNode, object operation)
        {
            this.leftNode = leftNode;
            this.rightNode = rightNode;
            this.operation = operation;
        }

        public ExpressionNode LeftNode { get => leftNode; }
        public ExpressionNode RightNode { get => rightNode; }
        public object Operation { get => operation; }
    }
}
