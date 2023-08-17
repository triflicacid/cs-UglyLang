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

        public override Signal Action(Context context, ISymbolContainer container)
        {
            Types.Type? type = CastType.Resolve(container);
            if (type == null)
            {
                context.Error = new(0, 0, Error.Types.Type, string.Format("failed to resolve '{0}' to a type", CastType));
                return Signal.ERROR;
            }

            return Symbol.CastValue(context, container, type) ? Signal.NONE : Signal.ERROR;
        }
    }
}
