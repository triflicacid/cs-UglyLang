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
        public string? Counter;

        public LoopKeywordNode(ExprNode? condition = null) : base("LOOP")
        {
            Body = null;
            Condition = condition;
            Counter = null;
        }

        public override Signal Action(Context context)
        {
            if (Body == null) throw new NullReferenceException(); // Should not be the case

            // Define counter, or set existing variable to 0
            bool counterIsFloat = false;
            if (Counter != null)
            {
                if (context.HasVariable(Counter))
                {
                    Value oldValue = context.GetVariable(Counter);
                    if (oldValue is IntValue intValue)
                    {
                        intValue.Value = 0;
                    }
                    else if (oldValue is FloatValue floatValue)
                    {
                        floatValue.Value = 0;
                        counterIsFloat = true;
                    }
                    else
                    {
                        context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("expected counter to be INT or FLOAT, got {0}", oldValue.Type.ToString()));
                        return Signal.ERROR;
                    }
                }
                else
                {
                    context.CreateVariable(Counter, new IntValue(0));
                }
            }

            while (true)
            {
                // If the loop has a condition, only continue if the condition is truthy
                if (Condition != null)
                {
                    Value value = Condition.Evaluate(context);
                    if (context.Error != null) // Propagate error?
                        return Signal.ERROR;

                    if (!value.IsTruthy())
                        break;
                }

                // Evaluate the loop body and handle the resulting signal
                Signal signal = Body.Evaluate(context);
                if (signal != Signal.NONE)
                {
                    if (signal == Signal.EXIT_LOOP) return Signal.NONE; // Signal has been processed.
                    return signal;
                }

                // Increment the counter variable, if present
                if (Counter != null)
                {
                    Value oldValue = context.GetVariable(Counter);
                    if (counterIsFloat)
                    {
                        ((FloatValue)oldValue).Value++;
                    }
                    else
                    {
                        ((IntValue)oldValue).Value++;
                    }
                }
            }

            return Signal.NONE;
        }
    }
}
