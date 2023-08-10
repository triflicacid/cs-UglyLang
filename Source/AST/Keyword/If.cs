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
    /// Evaluate an expression and throw away the result
    /// </summary>
    public class IfKeywordNode : KeywordNode
    {
        public readonly List<IfCondition> Conditions = new();
        public ASTStructure? Otherwise = null;
        public bool MetElseKeyword = false;

        public override Signal Action(Context context)
        {
            if (Conditions.Count == 0)
            {
                throw new NullReferenceException(); // Body should have been set
            }

            // Iterate through each condition, executing that block's body if truthy
            foreach (var item in Conditions)
            {
                if (item.Body == null) throw new NullReferenceException();

                Value? value = item.Condition.Evaluate(context);
                if (value == null) return Signal.ERROR; // Propagate error
                if (value.IsTruthy()) return item.Body.Evaluate(context);
            }

            return Otherwise == null ? Signal.NONE : Otherwise.Evaluate(context);
        }
    }

    public class IfCondition
    {
        public ExprNode Condition;
        public ASTStructure? Body = null;

        public IfCondition(ExprNode condition)
        {
            Condition = condition;
        }
    }
}
