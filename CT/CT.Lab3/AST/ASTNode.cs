using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.AST
{
    public abstract class ASTNode
    {
        public abstract object Value { get; }

        //public string Type() => typeof(this);
    }
}
