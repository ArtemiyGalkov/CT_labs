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

        public override object Value { get => identifierName; }

        //public override string Type()
        //{
        //    return "IdentifierNode";
        //}
    }
}
