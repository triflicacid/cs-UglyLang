using System;
using UglyLang.Source.Functions;
using UglyLang.Source.Types;
using UglyLang.Source.Values;
using static UglyLang.Source.Functions.Function;

namespace UglyLang.Source.AST
{
    public class TypeConstructNode : ASTNode
    {
        public UnresolvedType Construct;
        public List<ExprNode> Arguments;

        public TypeConstructNode(UnresolvedType construct)
        {
            Construct = construct;
            Arguments = new();
        }

        public override Value? Evaluate(Context context)
        {
            Types.Type? rawType = Construct.Resolve(context);
            if (rawType == null)
            {
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("failed to resolve '{0}' to a type", Construct.Value.GetSymbolString()));
                return null;
            }
                
            Types.Type type = rawType.ResolveParametersAgainst(context.GetBoundTypeParams());
            if (type.IsParameterised())
            {
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("parameterised type {0} cannot be resolved", rawType));
                return null;
            }

            if (!type.CanConstruct())
            {
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("type {0} cannot be constructed", type));
                return null;
            }

            // Evaluate arguments
            List<Value> arguments = new();
            foreach (ExprNode expr in Arguments)
            {
                Value? arg = expr.Evaluate(context);
                if (arg == null)
                    return null;
                if (context.Error != null)
                {
                    return null; // Propagate error
                }

                arguments.Add(arg);
            }

            // Construct the type
            (Signal s, Value? value) = type.Construct(context, arguments, LineNumber, ColumnNumber);
            if (s == Signal.ERROR) return null;

            return value;
        }
    }
}
