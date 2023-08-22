using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.AST
{
    public class TypeConstructNode : ASTNode
    {
        public UnresolvedType? Construct;
        public List<ExprNode> Arguments;

        public TypeConstructNode(UnresolvedType? construct = null)
        {
            Construct = construct;
            Arguments = new();
        }

        public override Value? Evaluate(Context context)
        {
            // Evaluate each argument
            List<Value> evaldArguments = new();
            Value? value;
            foreach (ExprNode node in Arguments)
            {
                value = node.Evaluate(context);
                if (value == null)
                    return null; // Propagate error
                evaldArguments.Add(value);
            }

            Types.Type? rawType; // resolved Construct type
            if (Construct == null)
            {
                // Construct is a list - determine member type from arguments
                ListType listType = new(evaldArguments[0].Type);

                for (int i = 1; i < evaldArguments.Count; i++)
                {
                    if (!evaldArguments[i].Type.Equals(listType.Member))
                    {
                        context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("expected {0}, got {1}, in {2} construct (argument {3})", listType.Member, evaldArguments[i].Type, listType, i + 1));
                        return null;
                    }
                }

                rawType = listType;
            }
            else
            {
                rawType = Construct.Resolve(context);

                if (rawType == null)
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("failed to resolve '{0}' to a type", Construct.Value.GetSymbolString()));
                    return null;
                }
            }

            Types.Type type = rawType.ResolveParametersAgainst(context.GetBoundTypeParams());
            if (type.IsParameterised())
            {
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("parameterised type {0} cannot be resolved", rawType));
                return null;
            }

            // Can the type be constructed?
            if (!type.CanConstruct())
            {
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("type {0} cannot be constructed", type));
                return null;
            }

            // Construct the type using the arguments, if necessary
            value = Arguments.Count == 0 ? type.ConstructNoArgs(context) : type.ConstructWithArgs(context, evaldArguments);

            // Propagate error or return
            if (value == null)
            {
                if (context.Error != null)
                {
                    context.Error.LineNumber = LineNumber;
                    context.Error.ColumnNumber = ColumnNumber;
                }

                return null;
            }
            else
            {
                return value;
            }
        }
    }
}
