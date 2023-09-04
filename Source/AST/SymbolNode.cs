using UglyLang.Source.Functions;
using UglyLang.Source.Types;
using UglyLang.Source.Values;
using static UglyLang.Source.Functions.Function;

namespace UglyLang.Source.AST
{
    /// <summary>
    /// A wrapper for node types which contain a symbol and require lookup in the symbol stack. This includes SymbolNode and ChainedSymbolNode.
    /// </summary>
    public abstract class AbstractSymbolNode : ASTNode
    {
        /// <summary>
        /// Return the string name that this symbol represents
        /// </summary>
        public abstract string GetSymbolString();

        /// <summary>
        /// Attempt to set this symbol to the given value in the given context. Cast only if the types match, or if forceCast is truthy. Return whether this was a success - see context.Error.
        /// </summary>
        public abstract bool SetValue(Context context, Value value, bool forceCast = false);

        /// <summary>
        /// Attempt to update the symbol's value. Return whether this was a success - see context.Error.
        /// </summary>
        public abstract bool UpdateValue(Context context, Func<Context, Value, Value?> transformer, bool forceCast = false);

        /// <summary>
        /// Attempt to cast this symbol to the given type. Return whether this was a success - see context.Error.
        /// </summary>
        public abstract bool CastValue(Context context, Types.Type type);
    }

    /// <summary>
    /// A node which contains a reference to a symbol.
    /// </summary>
    public class SymbolNode : AbstractSymbolNode
    {
        public readonly string Symbol;

        /// If not null then the symbol is a reference to a function (or should be)
        public List<ExprNode>? CallArguments;

        public SymbolNode(string symbol)
        {
            Symbol = symbol;
            CallArguments = null;
        }

        public override string GetSymbolString()
        {
            return Symbol;
        }

        public override Value? Evaluate(Context context)
        {
            if (context.HasSymbol(Symbol))
            {
                ISymbolValue variable = context.GetSymbol(Symbol);
                if (variable is Types.Type t) variable = new TypeValue(t);
                Value value;

                if (variable is ICallable callable)
                {
                    // Push new stack context
                    if (callable is Method method)
                    {
                        context.PushMethodStackContext(LineNumber, ColumnNumber, Symbol, method.Owner);
                    }
                    else
                    {
                        context.PushStackContext(LineNumber, ColumnNumber, StackContextType.Function, Symbol);
                    }

                    // Evaluate arguments
                    List<Value> arguments = new();
                    if (CallArguments != null)
                    {
                        foreach (ExprNode expr in CallArguments)
                        {
                            Value? arg = expr.Evaluate(context);
                            if (arg == null) return null;
                            if (context.Error != null) return null; // Propagate error

                            arguments.Add(arg);
                        }
                    }

                    // Call function with given arguments
                    Signal signal = callable.Call(context, arguments, LineNumber, ColumnNumber + Symbol.Length);
                    if (signal == Signal.ERROR)
                    {
                        return null;
                    }

                    // Fetch return value
                    value = context.GetFunctionReturnValue() ?? new EmptyValue();

                    // Pop stack context
                    context.PopStackContext();
                }
                else if (variable is Value val)
                {
                    if (val is TypeValue typeValue && CallArguments != null) // Construct the type
                    {
                        Types.Type type = typeValue.Value.ResolveParametersAgainst(context.GetBoundTypeParams());
                        if (type.IsParameterised())
                        {
                            context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("parameterised type {0} cannot be resolved", typeValue.Value));
                            return null;
                        }

                        if (!type.CanConstruct())
                        {
                            context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("type {0} cannot be constructed", type));
                            return null;
                        }

                        // Evaluate arguments
                        List<Value> arguments = new();
                        foreach (ExprNode expr in CallArguments)
                        {
                            Value? arg = expr.Evaluate(context);
                            if (arg == null)
                                return null;
                            if (context.Error != null)
                            {
                                return null; // Propagate error
                            }

                            arguments.Add(arg);
                        }

                        // Construct the type
                        (Signal s, Value? newValue) = type.Construct(context, arguments, LineNumber, ColumnNumber);
                        if (s == Signal.ERROR || newValue == null) return null;

                        value = newValue;
                    }
                    else if (CallArguments != null && CallArguments.Count != 0)
                    {
                        context.Error = new(LineNumber, ColumnNumber, Error.Types.Syntax, string.Format("value of type {0} is not callable", val.Type));
                        return null;
                    }
                    else
                    {
                        value = val;
                    }
                }
                else
                {
                    throw new InvalidOperationException(variable.GetType().Name);
                }

                return value;
            }
            else
            {
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Name, Symbol);
                return null;
            }
        }

        public override bool SetValue(Context context, Value value, bool forceCast = false)
        {
            // Make sure that the types line up
            if (context.HasSymbol(Symbol))
            {
                ISymbolValue oldSymbolValue = context.GetSymbol(Symbol);

                if (oldSymbolValue is Value oldValue)
                {
                    if (forceCast || oldValue.Type.DoesMatch(value.Type))
                    {
                        Value? newValue = value.To(oldValue.Type);
                        if (newValue == null)
                        {
                            context.Error = new(LineNumber, ColumnNumber, Error.Types.Cast, string.Format("casting {0} of type {1} to type {2}", Symbol, value.Type, oldValue.Type));
                            return false;
                        }
                        else
                        {
                            context.SetSymbol(Symbol, newValue);
                            return true;
                        }
                    }
                    else
                    {
                        context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("cannot match {0} with {1} (in assignment to {2})", value.Type.ToString(), oldValue.Type.ToString(), Symbol));
                        return false;
                    }
                }
                else
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("cannot set symbol '{0}' to type {1}", Symbol, value.Type));
                    return false;
                }
            }
            else
            {
                context.CreateSymbol(Symbol, value);
                return true;
            }
        }

        public override bool UpdateValue(Context context, Func<Context, Value, Value?> transformer, bool forceCast = false)
        {
            // Make sure that the types line up
            if (context.HasSymbol(Symbol))
            {
                ISymbolValue oldSymbolValue = context.GetSymbol(Symbol);

                if (oldSymbolValue is Value oldValue)
                {
                    // Transform the old value to new
                    Value? value = transformer(context, oldValue);
                    if (value == null) return false;

                    // Cast to match?
                    if (forceCast || oldValue.Type.DoesMatch(value.Type))
                    {
                        Value? newValue = value.To(oldValue.Type);
                        if (newValue == null)
                        {
                            context.Error = new(LineNumber, ColumnNumber, Error.Types.Cast, string.Format("casting {0} to type {1}", Symbol, oldValue.Type));
                            return false;
                        }
                        else
                        {
                            context.SetSymbol(Symbol, newValue);
                            return true;
                        }
                    }
                    else
                    {
                        context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("cannot match {0} with {1} (in assignment to {2})", value.Type.ToString(), oldValue.Type.ToString(), Symbol));
                        return false;
                    }
                }
                else
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("cannot set symbol {0}", Symbol));
                    return false;
                }
            }
            else
            {
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Name, Symbol);
                return false;
            }
        }

        public override bool CastValue(Context context, Types.Type type)
        {
            if (context.HasSymbol(Symbol))
            {
                ISymbolValue sValue = context.GetSymbol(Symbol);
                if (sValue is Value value)
                {
                    Value? newValue = value.To(type);
                    if (newValue == null)
                    {
                        context.Error = new(LineNumber, ColumnNumber, Error.Types.Cast, string.Format("casting {0} of type {1} to type {2}", Symbol, value.Type, type));
                        return false;
                    }
                    else
                    {
                        context.SetSymbol(Symbol, newValue);
                        return true;
                    }
                }
                else
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("cannot cast symbol '{0}'", Symbol));
                    return false;
                }
            }
            else
            {
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Name, Symbol);
                return false;
            }
        }
    }
}
