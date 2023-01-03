using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.AST
{
    public class IdentifierNode : ASTNode
    {
        private string identifierName;

        public IdentifierNode(string identifierName)
        {
            this.identifierName = identifierName;
        }

        public string Name { get => identifierName; }
    }
}
