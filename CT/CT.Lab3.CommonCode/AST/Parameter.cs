using CT.Lab3.AST;
using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.CommonCode.AST
{
    public class Parameter : Variable
    {
        private string type;

        public Parameter(string name, string type) : base(name)
        {
            this.type = type;
        }
    }
}
