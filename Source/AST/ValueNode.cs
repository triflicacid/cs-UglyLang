using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source;

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
            Type = ASTNodeType.VALUE;
            Value = value;
        }

        public override Value Evaluate(Context context)
        {
            return Value;
        }
    }
}
