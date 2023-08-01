using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source;

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

        public override Value Evaluate(Context context)
        {
            throw new Exception("Should not call Evaluate on this node");
        }

        public override Signal Action(Context context)
        {
            Value evaldValue = Value.Evaluate(context);
            if (context.Error != null) // Propagate error?
            {
                return Signal.ERROR;
            }

            context.SetVariable(Name, evaldValue);
            return Signal.NONE;
        }
    }
}
