using CT.Lab3.AST;
using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.CommonCode.AST
{
    public class ImportNode : ASTNode
    {
        private string importedModule;

        public ImportNode(string importedModule)
        {
            this.importedModule = importedModule;
        }
    }
}
