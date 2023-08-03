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
                Value oldValue = context.GetVariable(Name);
                if (Value.Match(oldValue.Type, evaldValue.Type))
                {
                    if (oldValue.Type != Values.ValueType.ANY)
                        evaldValue = evaldValue.To(oldValue.Type);
                }
                else
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("cannot match {0} with {1} (in assignment to {2})", evaldValue.Type.ToString(), oldValue.Type.ToString(), Name));
                    return Signal.ERROR;
                }
            }

            context.SetVariable(Name, evaldValue);
            return Signal.NONE;
        }
    }
}
