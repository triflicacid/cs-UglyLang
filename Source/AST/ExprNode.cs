using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.AST
{
    /// <summary>
    /// A node which contains an expression. An expression contains one node, or multiple nodes which will be concatenated as strings.
    /// </summary>
    public class ExprNode : ASTNode
    {
        public readonly List<ASTNode> Children;
        public UnresolvedType? CastType = null;

        public ExprNode()
        {
            Children = new();
        }

        public ExprNode(ASTNode child)
        {
            Children = new() { child };
        }

        public override Value? Evaluate(Context context)
        {
            Types.Type? castTo = null;
            if (CastType != null)
            {
                castTo = CastType.Resolve(context);
                if (castTo == null)
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("failed to resolve '{0}' to a type", CastType.Value.GetSymbolString()));
                    return null;
                }

                if (castTo.IsParameterised())
                {
                    castTo = castTo.ResolveParametersAgainst(context.GetBoundTypeParams());
                }
            }

            if (Children.Count == 0)
            {
                throw new InvalidOperationException();
            }
            else if (Children.Count == 1)
            {
                Value? value = Children[0].Evaluate(context);
                if (value == null)
                    return null;

                Value? newValue = castTo == null ? value : value.To(castTo);
                if (newValue == null)
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Cast, string.Format("cannot cast type {0} to {1}", value.Type, castTo));
                    return null;
                }

                return newValue;
            }
            else
            {
                List<Value> values = new();

                foreach (ASTNode child in Children)
                {
                    Value? value = child.Evaluate(context);
                    if (value == null)
                        return null;
                    values.Add(value);
                }

                // One of the first two values must be strings
                if (values[0].Type is not StringType && values[1].Type is not StringType)
                {
                    context.Error = new(Children[1].LineNumber, Children[1].ColumnNumber, Error.Types.Type, string.Format("expected at least one STRING, got {0} and {1} in concatenation expression", values[0].Type, values[1].Type));
                    return null;
                }

                // Concatenate to form a string
                string str = "";
                foreach (Value value in values)
                {
                    Value? sValue = value.To(Types.Type.StringT);
                    if (sValue == null)
                    {
                        context.Error = new(LineNumber, ColumnNumber, Error.Types.Cast, string.Format("cannot cast type {0} to STRING", value.Type));
                        return null;
                    }
                    else
                    {
                        str += ((StringValue)sValue).Value;
                    }
                }

                return new StringValue(str);
            }
        }
    }
}
