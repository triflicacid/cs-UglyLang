using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.source;

namespace UglyLang.Source.AST.Keyword
{
    /// <summary>
    /// Node for the LET keyword - create a new variable
    /// </summary>
    public class LetKeywordNode : KeywordNode
    {
        public readonly string Name;
        public readonly ExprNode Value;

        public LetKeywordNode(string name, ExprNode value) : base("LET")
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
            if (context.HasVariable(Name))
            {
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Name, string.Format("\"{0}\" already exists.", Name));
                return Signal.ERROR;
            }
            else
            {
                Value evaldValue = Value.Evaluate(context);
                if (context.Error != null) // Propagate error?
                {
                    return Signal.ERROR;
                }

                context.CreateVariable(Name, evaldValue);
                return Signal.NONE;
            }
        }
    }
}
