using CT.Lab3.AST;
using CT.Lab3.CommonCode.AST;
using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.Go.SpecificASTNodes
{
    public class MethodOfObjectDeclaration : MethodDeclarationNode
    {
        private (string objectName, string typeName)? appendingObject;

        public MethodOfObjectDeclaration(string methodName, string methodReturnType, (string objectName, string typeName)? appendingObject, Variable[] parameters, ProgramBlockNode methodBody) : base(methodName, methodReturnType, parameters, methodBody)
        {
            this.appendingObject = appendingObject;
        }
    }
}
