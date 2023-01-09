using CT.Lab3.AST;
using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.CommonCode
{
    public class BinaryTreeHelper
    {
        private ISyntaxProvider syntaxProvider;

        public BinaryTreeHelper(ISyntaxProvider syntaxProvider)
        {
            this.syntaxProvider = syntaxProvider;
        }

        public BinaryExpressionNode NormalizeExpressionTree(BinaryExpressionNode tree)
        {
            List<ExpressionNode> treeNodes = new List<ExpressionNode>();
            List<string> operations = new List<string>();

            InspectTreeNode(tree, treeNodes, operations);

            Stack<ExpressionNode> nodesStack = new Stack<ExpressionNode>();
            Stack<string> operatorsStack = new Stack<string>();

            int currentPriority = int.MaxValue;

            nodesStack.Push(treeNodes[0]);

            for (int i = 0; i < operations.Count; i++)
            {
                nodesStack.Push(treeNodes[i + 1]);
                operatorsStack.Push(operations[i]);

                if (currentPriority < GetOperatorPriority(operations[i]))
                {
                    PerformOperation(nodesStack, operatorsStack);

                    currentPriority = GetOperatorPriority(operatorsStack.Peek());
                }

                currentPriority = GetOperatorPriority(operatorsStack.Peek());
            }

            while (nodesStack.Count > 1)
            {
                PerformOperation(nodesStack, operatorsStack);
            }

            return nodesStack.Pop() as BinaryExpressionNode;
        }

        private void PerformOperation(Stack<ExpressionNode> nodesStack, Stack<string> operatorsStack)
        {
            var operation = operatorsStack.Pop();

            var rightNode = nodesStack.Pop();
            var leftNode = nodesStack.Pop();

            var resultingExpression = new BinaryExpressionNode(leftNode, rightNode, operation);
            nodesStack.Push(resultingExpression);
        }

        private int GetOperatorPriority(string operation)
            => syntaxProvider.OperatorsPriority[operation];

        private void InspectTreeNode(BinaryExpressionNode node, List<ExpressionNode> treeNodes, List<string> operations)
        {
            operations.Add(node.Operation.ToString());

            if (node.LeftNode is BinaryExpressionNode leftNode)
            {
                InspectTreeNode(leftNode, treeNodes, operations);
            }
            else
            {
                treeNodes.Add(node.LeftNode);
            }

            if (node.RightNode is BinaryExpressionNode rightNode)
            {
                InspectTreeNode(rightNode, treeNodes, operations);
            }
            else
            {
                treeNodes.Add(node.RightNode);
            }
        }
    }
}
