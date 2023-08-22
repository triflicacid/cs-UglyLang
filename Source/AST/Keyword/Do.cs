namespace UglyLang.Source.AST.Keyword
{
    /// <summary>
    /// Evaluate an expression and throw away the result
    /// </summary>
    public class DoKeywordNode : KeywordNode
    {
        public readonly ExprNode Expr;

        public DoKeywordNode(ExprNode expr)
        {
            Expr = expr;
        }

        public override Signal Action(Context context)
        {
            return Expr.Evaluate(context) == null ? Signal.ERROR : Signal.NONE;
        }
    }
}
