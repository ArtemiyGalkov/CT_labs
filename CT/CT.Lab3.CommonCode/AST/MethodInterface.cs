using CT.Lab3.AST;
using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.CommonCode.AST
{
    public class MethodInterface : ASTNode
    {
        private string methodName;
        private string methodReturnType;
        private Variable[] parameters;

        public MethodInterface(string methodName, string methodReturnType, Variable[] parameters)
        {
            this.methodName = methodName;
            this.methodReturnType = methodReturnType;
            this.parameters = parameters;
        }

        public string MethodName { get => methodName; }
        public string ReturnType { get => methodReturnType; }
        public Variable[] Parameters { get => parameters; }
    }
}
