using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source;
using UglyLang.Source.Values;

namespace UglyLang.Source.AST.Keyword
{
    /// <summary>
    /// Node for the PRINT keyword - print a value to the screen
    /// </summary>
    public class PrintKeywordNode : KeywordNode
    {
        public readonly ExprNode Expr;
        public bool Newline;

        public PrintKeywordNode(ExprNode expr, bool newline = false) : base("PRINT")
        {
            Expr = expr;
            Newline = newline;
        }

        public override Signal Action(Context context)
        {
            Value? value = Expr.Evaluate(context);
            if (value == null) // Propagate error?
                return Signal.ERROR;

            string str = StringValue.From(value).Value;
            if (Newline)
            {
                Console.WriteLine(str);
            }
            else
            {
                Console.Write(str);
            }
            
            return Signal.NONE;
        }
    }
}
