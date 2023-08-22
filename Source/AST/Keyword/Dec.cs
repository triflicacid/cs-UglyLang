using UglyLang.Source.Values;

namespace UglyLang.Source.AST.Keyword
{
    /// <summary>
    /// Increment the given symbol
    /// </summary>
    public class DecKeywordNode : KeywordNode
    {
        public readonly AbstractSymbolNode Symbol;

        public DecKeywordNode(AbstractSymbolNode symbol)
        {
            Symbol = symbol;
        }

        public override Signal Action(Context context)
        {
            return Symbol.UpdateValue(context, Increment, true) ? Signal.NONE : Signal.ERROR;
        }

        private Value? Increment(Context context, Value value)
        {
            if (value is IntValue iValue)
            {
                return new IntValue(iValue.Value - 1);
            }
            else if (value is FloatValue fValue)
            {
                return new FloatValue(fValue.Value - 1);
            }
            else
            {
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("expected INT or FLOAT, got {0}", value.Type));
                return null;
            }
        }
    }
}
