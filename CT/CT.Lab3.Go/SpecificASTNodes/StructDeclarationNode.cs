using CT.Lab3.AST;
using CT.Lab3.CommonCode.AST;
using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.Go.SpecificASTNodes
{
    public class StructDeclarationNode : TypeDeclaration
    {
        public StructDeclarationNode(string typeName, VariableInitializationNode[] typeFields) : base(typeName, typeFields)
        {
        }
    }
}
