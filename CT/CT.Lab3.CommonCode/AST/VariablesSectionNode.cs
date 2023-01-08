using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.AST
{
    public class VariablesSectionNode : ASTNode
    {
        private VariableDeclarationNode[] variableDeclarationNodes;

        public VariablesSectionNode(VariableDeclarationNode[] variableDeclarationNodes)
        {
            this.variableDeclarationNodes = variableDeclarationNodes;
        }

        public VariableDeclarationNode[] VariableDeclarationNodes
        {
            get => variableDeclarationNodes;
        }
    }
}
