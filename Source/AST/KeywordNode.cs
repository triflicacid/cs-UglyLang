using UglyLang.Source.Values;

namespace UglyLang.Source.AST
{
    /// <summary>
    /// Node containing the concept of a keyword/action
    /// </summary>
    public abstract class KeywordNode : ASTNode
    {
        public override Value Evaluate(Context context, ISymbolContainer container)
        {
            throw new NotImplementedException();
        }
    }
}
