using UglyLang.Source.Types;

namespace UglyLang.Source.AST.Keyword
{
    /// <summary>
    /// Node for the LET keyword - create a new variable
    /// </summary>
    public class CastKeywordNode : KeywordNode
    {
        public readonly AbstractSymbolNode Symbol;
        public readonly UnresolvedType CastType;

        public CastKeywordNode(AbstractSymbolNode symbol, UnresolvedType type)
        {
            Symbol = symbol;
            CastType = type;
        }

        public override Signal Action(Context context)
        {
            Types.Type? type = CastType.Resolve(context);
            if (type == null)
            {
                context.Error = new(0, 0, Error.Types.Type, string.Format("failed to resolve '{0}' to a type", CastType));
                return Signal.ERROR;
            }

            return Symbol.CastValue(context, type) ? Signal.NONE : Signal.ERROR;

            /*
            if (context.HasVariable(Symbol))
            {
                Types.Type? type = CastType.Resolve(context);
                if (type == null)
                {
                    context.Error = new(0, 0, Error.Types.Type, string.Format("failed to resolve '{0}' to a type", CastType));
                    return Signal.ERROR;
                }

                ISymbolValue sValue = context.GetVariable(Symbol);
                if (sValue is Value value)
                {
                    Value? newValue = value.To(type);
                    if (newValue == null)
                    {
                        context.Error = new(LineNumber, ColumnNumber, Error.Types.Cast, string.Format("casting {0} of type {1} to type {2}", Symbol, value.Type, type));
                        return Signal.ERROR;
                    }
                    else
                    {
                        context.SetVariable(Symbol, newValue);
                        return Signal.NONE;
                    }
                }
                else
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("cannot cast symbol '{0}'", Symbol));
                    return Signal.ERROR;
                }
            }
            else
            {
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Name, Symbol);
                return Signal.ERROR;
            }
            */
        }
    }
}
