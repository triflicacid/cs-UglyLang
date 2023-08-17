using UglyLang.Source.Values;

namespace UglyLang.Source.AST
{
    /// <summary>
    /// Node which is a wrapper for a string. Should not be used at runtime!
    /// </summary>
    public class StringNode : ASTNode
    {
        public string Value;

        public StringNode(string s)
        {
            Value = s;
        }

        public override Value Evaluate(Context context)
        {
            throw new NotSupportedException();
        }
    }
}
