using UglyLang.Source.Values;

namespace UglyLang.Source.AST.Keyword
{
    /// <summary>
    /// Node for the LET keyword - create a new variable
    /// </summary>
    public class LetKeywordNode : KeywordNode
    {
        public readonly string Name;
        public readonly ExprNode Value;

        public LetKeywordNode(string name, ExprNode value)
        {
            Name = name;
            Value = value;
        }

        public override Signal Action(Context context, ISymbolContainer container)
        {
            if (container.HasSymbol(Name))
            {
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Name, string.Format("\"{0}\" is already defined.", Name));
                return Signal.ERROR;
            }
            else
            {
                Value? evaldValue = Value.Evaluate(context, container);
                if (evaldValue == null) // Propagate error?
                {
                    return Signal.ERROR;
                }

                container.CreateSymbol(Name, evaldValue);
                return Signal.NONE;
            }
        }
    }
}
