using CT.Lab3.AST;
using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.BASIC.SpecificASTNodes
{
    public class PrintNode : ASTNode
    {
        private string printMessage;
        private string variableName;

        public PrintNode(string printMessage, string? variableName)
        {
            this.printMessage = printMessage;
            this.variableName = variableName;
        }

        public string PrintMessage => printMessage;
        public string PrintingVariable => variableName;
    }
}
