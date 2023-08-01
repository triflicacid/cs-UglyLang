using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source;

namespace UglyLang.Source.AST.Keyword
{
    /// <summary>
    /// Node for the PRINT keyword - print a value to the screen
    /// </summary>
    public class PrintKeywordNode : KeywordNode
    {
        public readonly ExprNode Expr;

        public PrintKeywordNode(ExprNode expr) : base("PRINT")
        {
            Expr = expr;
        }

        public override Value Evaluate(Context context)
        {
            throw new Exception("Should not call Evaluate on this node");
        }

        public override Signal Action(Context context)
        {
            Value value = Expr.Evaluate(context);
            if (context.Error != null) // Propagate error?
                return Signal.ERROR;

            Console.WriteLine(StringValue.From(value).Value);
            return Signal.NONE;
        }
    }
}
