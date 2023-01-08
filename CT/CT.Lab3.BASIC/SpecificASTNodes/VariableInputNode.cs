using CT.Lab3.AST;
using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.BASIC.SpecificASTNodes
{
    public class VariableInputNode : ASTNode
    {
        private string assigmentMessage;
        private string variableName;

        public VariableInputNode(string assigmentMessage, string variableName)
        {
            this.assigmentMessage = assigmentMessage;
            this.variableName = variableName;
        }

        public string AssigmentMessage => assigmentMessage;
        public string VariableName => variableName;
    }
}
