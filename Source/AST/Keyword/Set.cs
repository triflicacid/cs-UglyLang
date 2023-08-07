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
    /// Node for the LET keyword - create a new variable
    /// </summary>
    public class SetKeywordNode : KeywordNode
    {
        public readonly string Name;
        public readonly ExprNode Expr;

        public SetKeywordNode(string name, ExprNode expr) : base("SET")
        {
            Name = name;
            Expr = expr;
        }

        public override Signal Action(Context context)
        {
            Value evaldValue = Expr.Evaluate(context);
            if (context.Error != null) // Propagate error?
            {
                return Signal.ERROR;
            }

            // Make sure that the types line up
            if (context.HasVariable(Name))
            {
                ISymbolValue oldSymbolValue = context.GetVariable(Name);
                if (oldSymbolValue is Value oldValue)
                {
                    if (oldValue.Type.DoesMatch(evaldValue.Type))
                    {
                        Value? newValue = evaldValue.To(oldValue.Type);
                        if (newValue == null)
                        {
                            context.Error = new(LineNumber, ColumnNumber, Error.Types.Cast, string.Format("casting {0} to type {1}", Name, oldValue.Type));
                            return Signal.ERROR;
                        }
                        else
                        {
                            evaldValue = newValue;
                        }
                    }
                    else
                    {
                        context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("cannot match {0} with {1} (in assignment to {2})", evaldValue.Type.ToString(), oldValue.Type.ToString(), Name));
                        return Signal.ERROR;
                    }
                }
                else
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("cannot set symbol '{0}' to type {1}", Name, evaldValue.Type));
                    return Signal.ERROR;
                }
            }

            context.SetVariable(Name, evaldValue);
            return Signal.NONE;
        }
    }
}
