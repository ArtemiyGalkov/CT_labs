using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.AST
{
    public class Variable : ExpressionNode
    {
        private string name;

        public Variable(string name)
        {
            this.name = name;
        }

        public string Name => name;
    }
}
