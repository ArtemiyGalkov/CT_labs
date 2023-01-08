using CT.Lab3.AST;
using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.CommonCode.AST
{
    public class InterfaceDeclaration : TypeDeclaration
    {
        public InterfaceDeclaration(string typeName, MethodInterface[] typeFields) : base(typeName, typeFields)
        {
        }
    }
}
