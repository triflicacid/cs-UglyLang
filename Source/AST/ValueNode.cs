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
