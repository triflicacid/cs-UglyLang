using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.AST.Keyword
{
    /// <summary>
    /// Raise an error
    /// </summary>
    public class ErrorKeywordNode : KeywordNode
    {
        public readonly ExprNode? Expr;

        public ErrorKeywordNode(ExprNode? expr = null)
        {
            Expr = expr;
        }

        public override Signal Action(Context context)
        {
            Error.Types errType;
            string errString;

            if (Expr == null)
            {
                errType = Error.Types.Raised;
                errString = "an error was raised";
            }
            else
            {
                Value? errVal = Expr.Evaluate(context);
                if (errVal == null)
                {
                    return Signal.ERROR; // Propagate
                }
                else
                {
                    Value? errStringVal = errVal.To(new StringType());
                    if (errStringVal == null)
                    {
                        errType = Error.Types.Cast;
                        errString = string.Format("casting {0} to {1}", errVal.Type, "STRING");
                    }
                    else
                    {
                        errType = Error.Types.Raised;
                        errString = ((StringValue)errStringVal).Value;
                    }
                }
            }

            context.Error = new(LineNumber, ColumnNumber, errType, errString);
            return Signal.ERROR;
        }
    }
}
