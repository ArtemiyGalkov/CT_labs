using CT.Lab3.AST;
using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.CommonCode.AST
{
    public class TypeDeclaration : ASTNode
    {
        private string typeName;
        private ASTNode[] typeFields;

        public TypeDeclaration(string typeName, ASTNode[] typeFields)
        {
            this.typeName = typeName;
            this.typeFields = typeFields;
        }
    }
}
