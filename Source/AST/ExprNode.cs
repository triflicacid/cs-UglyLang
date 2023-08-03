using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source;
using UglyLang.Source.Values;

namespace UglyLang.Source.AST
{
    /// <summary>
    /// A node which contains an expression. An expression contains one node, or multiple nodes which will be concatenated as strings.
    /// </summary>
    public class ExprNode : ASTNode
    {
        public readonly List<ASTNode> Children;
        public Values.ValueType? CastType = null;

        public ExprNode()
        {
            Type = ASTNodeType.EXPR;
            Children = new();
        }

        public ExprNode(ASTNode child)
        {
            Type = ASTNodeType.EXPR;
            Children = new() { child };
        }

        public override Value Evaluate(Context context)
        {
            if (Children.Count == 0)
            {
                throw new InvalidOperationException();
            }
            else if (Children.Count == 1)
            {
                Value value = Children[0].Evaluate(context);
                return CastType == null ? value : value.To((Values.ValueType)CastType);
            }
            else
            {
                string str = "";
                Value value;
                foreach (ASTNode child in Children)
                {
                    value = child.Evaluate(context);
                    str += ((StringValue)value.To(Values.ValueType.STRING)).Value;
                }
                value = new StringValue(str);
                return CastType == null ? value : value.To((Values.ValueType)CastType);
            }
        }
    }
}
