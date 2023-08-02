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
    /// A node which contains an expression. This class is only a semantic wrapper for another node.
    /// </summary>
    public class ExprNode : ASTNode
    {
        public ASTNode Child;

        public ExprNode(ASTNode child)
        {
            Type = ASTNodeType.EXPR;
            Child = child;
        }

        public override Value Evaluate(Context context)
        {
            Value value = Child.Evaluate(context);
            return CastType == null ? value : value.To((Values.ValueType)CastType);
        }
    }
}
