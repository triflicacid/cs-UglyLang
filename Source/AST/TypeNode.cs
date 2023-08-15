using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.AST
{
    /// <summary>
    /// Node which contains an unresolved type
    /// </summary>
    public class TypeNode : ASTNode
    {
        public UnresolvedType Value;

        public TypeNode(UnresolvedType value)
        {
            Value = value;
        }

        public override Value? Evaluate(Context context)
        {
            Types.Type? type = Value.Resolve(context);
            if (type == null)
            {
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("failed to resolve '{0}' to a type", Value.Value));
                return null;
            }
            else
            {
                // If parameterised, resolve bound parameters. Error if it is still parameterised.
                if (type.IsParameterised())
                {
                    type = type.ResolveParametersAgainst(context.GetBoundTypeParams());

                    if (type.IsParameterised())
                    {
                        context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("type literal cannot be parameterised: @{0}", type));
                        return null;
                    }
                }

                return new TypeValue(type);
            }
        }
    }
}
