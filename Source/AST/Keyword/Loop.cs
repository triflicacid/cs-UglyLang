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

        public LoopKeywordNode(ExprNode? condition = null)
        {
            Body = null;
            Condition = condition;
            Counter = null;
        }

        public override Signal Action(Context context, ISymbolContainer container)
        {
            if (Body == null)
                throw new NullReferenceException(); // Should not be the case

            // Define counter, or set existing variable to 0
            bool counterIsFloat = false;
            if (Counter != null)
            {
                if (container.HasSymbol(Counter))
                {
                    ISymbolValue oldValue = container.GetSymbol(Counter);
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
                        context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("expected counter to be INT or FLOAT, got {0}", oldValue is Value val ? val.Type : "unknown"));
                        return Signal.ERROR;
                    }
                }
                else
                {
                    container.CreateSymbol(Counter, new IntValue(0));
                }
            }

            while (true)
            {
                // If the loop has a condition, only continue if the condition is truthy
                if (Condition != null)
                {
                    Value? value = Condition.Evaluate(context, container);
                    if (value == null) // Propagate error?
                        return Signal.ERROR;

                    if (!value.IsTruthy())
                        break;
                }

                // Evaluate the loop body and handle the resulting signal
                Signal signal = Body.Evaluate(context, container);
                if (signal != Signal.NONE)
                {
                    if (signal == Signal.EXIT_LOOP)
                        return Signal.NONE; // Signal has been processed.
                    return signal;
                }

                // Increment the counter variable, if present
                if (Counter != null)
                {
                    Value oldValue = (Value)container.GetSymbol(Counter);
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
