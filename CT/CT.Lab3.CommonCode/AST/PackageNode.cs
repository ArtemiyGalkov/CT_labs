using CT.Lab3.AST;
using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.CommonCode.AST
{
    public class PackageNode : ASTNode
    {
        private string packageName;

        public PackageNode(string packageName)
        {
            this.packageName = packageName;
        }
    }
}
