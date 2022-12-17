using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.AST
{
    public class ModuleDeclarationNode : ASTNode
    {
        private string moduleName;

        public ModuleDeclarationNode(string moduleName)
        {
            this.moduleName = moduleName;
        }

        public override object Value { get => moduleName; }
    }
}
