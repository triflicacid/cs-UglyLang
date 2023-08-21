using UglyLang.Source.Values;

namespace UglyLang.Source.AST.Keyword
{
    /// <summary>
    /// Increment the given symbol
    /// </summary>
    public class IncKeywordNode : KeywordNode
    {
        public readonly AbstractSymbolNode Symbol;

        public IncKeywordNode(AbstractSymbolNode symbol)
        {
            Symbol = symbol;
        }

        public override Signal Action(Context context, ISymbolContainer container)
        {
            return Symbol.UpdateValue(context, container, Increment, true) ? Signal.NONE : Signal.ERROR;
        }

        private Value? Increment(Context context, Value value)
        {
            if (value is IntValue iValue)
            {
                return new IntValue(iValue.Value + 1);
            }
            else if (value is FloatValue fValue)
            {
                return new FloatValue(fValue.Value + 1);
            }
            else
            {
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("expected INT or FLOAT, got {0}", value.Type));
                return null;
            }
        }
    }
}
