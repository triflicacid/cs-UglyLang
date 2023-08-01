using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.source;

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
            return Child.Evaluate(context);
        }
    }
}
