using CT.Lab3.AST;
using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.BASIC.SpecificASTNodes
{
    public class GoToNode : ASTNode
    {
        private int goToLine;

        public GoToNode(int goToLine)
        {
            this.goToLine = goToLine;
        }

        public int GoToLine => goToLine;
    }
}
