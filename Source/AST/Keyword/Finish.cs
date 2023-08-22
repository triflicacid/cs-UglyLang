using UglyLang.Source.Values;

namespace UglyLang.Source.AST.Keyword
{
    /// <summary>
    /// Exit (and return) from current function
    /// </summary>
    public class FinishKeywordNode : KeywordNode
    {
        public ExprNode? ReturnOnExit;

        public FinishKeywordNode(ExprNode? expr = null)
        {
            ReturnOnExit = expr;
        }

        public override Signal Action(Context context)
        {
            if (ReturnOnExit == null)
            {
                context.SetFunctionReturnValue(new EmptyValue());
            }
            else
            {
                Value? value = ReturnOnExit.Evaluate(context);
                if (value == null)
                    return Signal.ERROR;

                context.SetFunctionReturnValue(value);
            }

            return Signal.EXIT_FUNC;
        }
    }
}
