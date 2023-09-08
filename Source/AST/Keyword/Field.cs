using UglyLang.Source.Types;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.AST.Keyword
{
    /// <summary>
    /// Represents a named field for a custom type
    /// </summary>
    public class FieldKeywordNode : KeywordNode
    {
        public readonly string Name;
        public readonly UnresolvedType Type;

        public FieldKeywordNode(string name, UnresolvedType type)
        {
            Name = name;
            Type = type;
        }

        public override Signal Action(Context context)
        {
            UserTypeDataContainer data = (UserTypeDataContainer)context.PeekStack();
            if (data.Fields.ContainsKey(Name))
            {
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Name, string.Format("duplicate definition for field {0} in type {1}", Name, data.Name));
                return Signal.ERROR;
            }
            else
            {
                Type? type = Type.Resolve(context);
                if (type == null)
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("cannot resolve {0} to a type", Type.Value.GetSymbolString()));
                    return Signal.ERROR;
                }

                // Resolve the type
                if (type.IsParameterised())
                {
                    Type resolved = type.ResolveParametersAgainst(context.GetBoundTypeParams());

                    if (resolved.IsParameterised())
                    {
                        context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("cannot resolve parameterised type '{0}' (partially resolved to {1})", type, resolved));
                        return Signal.ERROR;
                    }

                    type = resolved;
                }

                data.Fields.Add(Name, new(Name, type)
                {
                    LineNumber = LineNumber,
                    ColumnNumber = ColumnNumber
                });
                return Signal.NONE;
            }
        }
    }
}
