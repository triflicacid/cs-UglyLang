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
        public readonly ExprNode Value;

        public SetKeywordNode(string name, ExprNode value) : base("SET")
        {
            Name = name;
            Value = value;
        }

        public override Signal Action(Context context)
        {
            Value evaldValue = Value.Evaluate(context);
            if (context.Error != null) // Propagate error?
            {
                return Signal.ERROR;
            }

            // Make sure that the types line up
            if (context.HasVariable(Name))
            {
                Value oldValue = context.GetVariable(Name);
                if (oldValue.Type != evaldValue.Type)
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
