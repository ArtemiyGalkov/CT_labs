using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.AST
{
    public class VariableDeclarationNode : ASTNode
    {
        public string[] variablesName;
        public string type;

        public VariableDeclarationNode(string[] variablesName, string type)
        {
            this.variablesName = variablesName;
            this.type = type;
        }

        public string[] VariablesNames => variablesName;

        public string Type => type;
    }
}
