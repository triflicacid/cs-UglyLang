using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.AST
{
    public class TypeConstructNode : ASTNode
    {
        public readonly UnresolvedType Construct;
        public readonly List<ExprNode> Arguments;

        public TypeConstructNode(UnresolvedType construct)
        {
            Construct = construct;
            Arguments = new();
            Type = ASTNodeType.TYPE_CONSTRUCT;
        }

        public override Value? Evaluate(Context context)
        {
            Types.Type? rawType = Construct.Resolve(context);
            if (rawType == null)
            {
                context.Error = new(0, 0, Error.Types.Type, string.Format("failed to resolve '{0}' to a type", Construct.Value));
                return null;
            }

            Types.Type type = rawType.ResolveParametersAgainst(context.GetBoundTypeParams());
            if (type.IsParameterised())
            {
                context.Error = new(0, 0, Error.Types.Type, string.Format("parameterised type {0} cannot be resolved", rawType));
                return null;
            }

            // Can the type be constructed?
            if (!type.CanConstruct())
            {
                context.Error = new(0, 0, Error.Types.Type, string.Format("type {0} cannot be constructed", type));
                return null;
            }

            Value? value;

            if (Arguments.Count == 0)
            {
                value = type.ConstructNoArgs(context);
            }
            else
            {
                // Evaluate each argument
                List<Value> evaldArguments = new();
                foreach (ExprNode node in Arguments)
                {
                   value = node.Evaluate(context);
                    if (value == null) return null; // Propagate error
                    evaldArguments.Add(value);
                }

                value = type.ConstructWithArgs(context, evaldArguments);
            }

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
