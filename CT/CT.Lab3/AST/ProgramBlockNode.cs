using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.AST
{
    public class ProgramBlockNode : ASTNode
    {
        private ASTNode[] nodes;

        public ProgramBlockNode(ASTNode[] nodes)
        {
            this.nodes = nodes;
        }

        public override object Value => throw new NotImplementedException();
    }
}
