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
                    context.Error = new(0, 0, Error.Types.Type, string.Format("failed to resolve '{0}' to a type", CastType.Value));
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
                if (value == null) return null;

                Value? newValue = castTo == null ? value : value.To(castTo);
                if (newValue == null)
                {
                    context.Error = new(0, 0, Error.Types.Cast, string.Format("cannot cast type {0} to {1}", value.Type, castTo));
                    return null;
                }

                return newValue;
            }
            else
            {
                string str = "";
                Value? value;

                foreach (ASTNode child in Children)
                {
                    value = child.Evaluate(context);
                    if (value == null) return null;

                    Value? newValue = value.To(new StringType());
                    if (newValue == null)
                    {
                        context.Error = new(0, 0, Error.Types.Cast, string.Format("cannot cast type {0} to STRING", value.Type));
                        return null;
                    }

                    str += newValue == null ? "" : ((StringValue)newValue).Value;
                }

                return new StringValue(str);
            }
        }
    }
}
