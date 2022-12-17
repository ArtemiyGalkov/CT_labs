using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.AST
{
    public class VariableDeclarationNode : ASTNode
    {
        public string variableName;
        public string type;

        public VariableDeclarationNode(string variableName, string type)
        {
            this.variableName = variableName;
            this.type = type;
        }

        public override object Value => variableName;

        public string Type => type;
    }
}
