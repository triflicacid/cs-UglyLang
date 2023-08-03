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
    /// Execute the body continuously while the condition is truthy
    /// </summary>
    public class WhileKeywordNode : KeywordNode
    {
        public readonly ExprNode Condition;
        public ASTStructure? Body;

        public WhileKeywordNode(ExprNode cond) : base("WHILE")
        {
            Condition = cond;
            Body = null;
        }

        public override Signal Action(Context context)
        {
            if (Body == null) throw new NullReferenceException(); // Should not be the case

            while (true)
            {
                Value value = Condition.Evaluate(context);
                if (context.Error != null) // Propagate error?
                    return Signal.ERROR;

                if (!value.IsTruthy()) break;

                Signal signal = Body.Evaluate(context);
                if (signal != Signal.NONE)
                {
                    if (signal == Signal.EXIT_LOOP) return Signal.NONE; // Signal has been processed.
                    return signal;
                }
            }

            return Signal.NONE;
        }
    }
}
