using UglyLang.Source.Values;

namespace UglyLang.Source.AST.Keyword
{
    /// <summary>
    /// Node for the PRINT keyword - print a value to the screen
    /// </summary>
    public class PrintKeywordNode : KeywordNode
    {
        public readonly ExprNode Expr;
        public bool Newline;

        public PrintKeywordNode(ExprNode expr, bool newline = false)
        {
            Expr = expr;
            Newline = newline;
        }

        public override Signal Action(Context context, ISymbolContainer container)
        {
            Value? value = Expr.Evaluate(context, container);
            if (value == null) // Propagate error?
                return Signal.ERROR;

            string str = StringValue.From(value).Value;
            if (Newline)
            {
                Console.WriteLine(str);
            }
            else
            {
                Console.Write(str);
            }

            return Signal.NONE;
        }
    }
}
