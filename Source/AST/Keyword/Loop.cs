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
    /// Execute the body. If there is a condition, loop while this is truthy, else loop conditionaly.
    /// </summary>
    public class LoopKeywordNode : KeywordNode
    {
        public ExprNode? Condition;
        public ASTStructure? Body;

        public LoopKeywordNode(ExprNode? condition = null) : base("LOOP")
        {
            Body = null;
            Condition = condition;
        }

        public override Signal Action(Context context)
        {
            if (Body == null) throw new NullReferenceException(); // Should not be the case

            if (Condition == null)
            {
                // Indefinite loop
                while (true)
                {
                    Signal signal = Body.Evaluate(context);
                    if (signal != Signal.NONE)
                    {
                        if (signal == Signal.EXIT_LOOP) return Signal.NONE; // Signal has been processed.
                        if (signal == Signal.EXIT_FUNC) return Signal.EXIT_FUNC; // Propagate signal
                        return signal;
                    }
                }
            }
            else
            {
                // Loop while the condition is truthy
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
                        if (signal == Signal.EXIT_FUNC) return Signal.EXIT_FUNC; // Propagate signal
                        return signal;
                    }
                }

                return Signal.NONE;
            }
        }
    }
}
