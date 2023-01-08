using CT.Lab3.AST;
using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.CommonCode.AST
{
    public class MethodDeclarationNode : MethodInterface
    {
        private ProgramBlockNode methodBody;

        public MethodDeclarationNode(string methodName, string methodReturnType, Variable[] parameters, ProgramBlockNode methodBody) 
            : base(methodName, methodReturnType, parameters)
        {
            this.methodBody = methodBody;
        }

        public ProgramBlockNode MethodBody { get => methodBody; }
    }
}
