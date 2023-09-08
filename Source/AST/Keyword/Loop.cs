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

        public override Signal Action(Context context)
        {
            if (Body == null)
                throw new NullReferenceException(); // Should not be the case

            // Define counter, or set existing variable to 0
            bool counterIsFloat = false;
            if (Counter != null)
            {
                if (context.HasSymbol(Counter))
                {
                    var variable = context.GetSymbol(Counter);
                    if (variable.IsReadonly)
                    {
                        context.Error = new(LineNumber, ColumnNumber, Error.Types.Name, string.Format("symbol '{0}' is read-only", Counter))
                        {
                            AppendString = string.Format("Symbol {0} was defined at {1}:{2}", Counter, variable.GetLineNumber() + 1, variable.GetColumnNumber() + 1),
                            AdditionalSource = ((ILocatable)variable).GetLocation()
                        };
                        return Signal.ERROR;
                    }
                    else if (variable.GetValue() is IntValue intValue)
                    {
                        intValue.Value = 0;
                    }
                    else if (variable.GetValue() is FloatValue floatValue)
                    {
                        floatValue.Value = 0;
                        counterIsFloat = true;
                    }
                    else
                    {
                        context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("expected counter to be INT or FLOAT, got {0}", variable.GetValue() is Value val ? val.Type : "unknown"));
                        return Signal.ERROR;
                    }
                }
                else
                {
                    context.CreateSymbol(new(Counter, new IntValue(0)));
                }
            }

            while (true)
            {
                // If the loop has a condition, only continue if the condition is truthy
                if (Condition != null)
                {
                    Value? value = Condition.Evaluate(context);
                    if (value == null) // Propagate error?
                        return Signal.ERROR;

                    if (!value.IsTruthy())
                        break;
                }

                // Evaluate the loop body and handle the resulting signal
                Signal signal = Body.Evaluate(context);
                if (signal != Signal.NONE)
                {
                    if (signal == Signal.EXIT_LOOP)
                        return Signal.NONE; // Signal has been processed.
                    return signal;
                }

                // Increment the counter variable, if present
                if (Counter != null)
                {
                    Value oldValue = (Value)context.GetSymbol(Counter).GetValue();
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
