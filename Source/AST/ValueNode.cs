using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source;
using UglyLang.Source.Values;

namespace UglyLang.Source.AST
{
    /// <summary>
    /// Node which contains a literal value
    /// </summary>
    public class ValueNode : ASTNode
    {
        public Value Value;

        public ValueNode(Value value)
        {
            Value = value;
        }

        public override Value Evaluate(Context context)
        {
            return Value;
        }
    }
}
