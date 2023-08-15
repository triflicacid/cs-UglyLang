using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UglyLang.Source;
using UglyLang.Source.Functions;
using UglyLang.Source.Values;

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
        /// Attempt to set this symbol to the given value in the given context. Return whether this was a success - see context.Error.
        /// </summary>
        public abstract bool SetValue(Context context, Value value);

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
            if (context.HasVariable(Symbol))
            {
                ISymbolValue variable = context.GetVariable(Symbol);
                Value value;

                if (variable is ICallable func)
                {
                    // Push new stack context
                    context.PushStackContext(LineNumber, ColumnNumber, Context.StackContext.Types.Function, Symbol);

                    // Evaluate arguments
                    List<Value> arguments = new();
                    if (CallArguments != null)
                    {
                        foreach (ExprNode expr in CallArguments)
                        {
                            Value? arg = expr.Evaluate(context);
                            if (arg == null) return null;
                            if (context.Error != null)
                            {
                                return null; // Propagate error
                            }

                            arguments.Add(arg);
                        }
                    }

                    // Call function with given arguments
                    Signal signal = func.Call(context, arguments);
                    if (signal == Signal.ERROR)
                    {
                        if (context.Error != null)
                        {
                            context.Error.LineNumber = LineNumber;
                            context.Error.ColumnNumber = ColumnNumber + Symbol.Length;
                        }

                        return null;
                    }

                    // Fetch return value
                    value = context.GetFunctionReturnValue() ?? new EmptyValue();

                    // Pop stack context
                    context.PopStackContext();
                }
                else if (variable is Value val)
                {
                    if (CallArguments != null && CallArguments.Count != 0)
                    {
                        context.Error = new(LineNumber, ColumnNumber, Error.Types.Syntax, string.Format("value of type {0} is not callable", val.Type));
                        return null;
                    }

                    value = val;
                }
                else
                {
                    throw new InvalidOperationException();
                }

                return value;
            }
            else
            {
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Name, Symbol);
                return null;
            }
        }

        public override bool SetValue(Context context, Value value)
        {
            // Make sure that the types line up
            if (context.HasVariable(Symbol))
            {
                ISymbolValue oldSymbolValue = context.GetVariable(Symbol);

                if (oldSymbolValue is Value oldValue)
                {
                    if (oldValue.Type.DoesMatch(value.Type))
                    {
                        Value? newValue = value.To(oldValue.Type);
                        if (newValue == null)
                        {
                            context.Error = new(LineNumber, ColumnNumber, Error.Types.Cast, string.Format("casting {0} to type {1}", Symbol, oldValue.Type));
                            return false;
                        }
                        else
                        {
                            context.SetVariable(Symbol, newValue);
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
                context.CreateVariable(Symbol, value);
                return true;
            }
        }

        public override bool CastValue(Context context, Types.Type type)
        {
            if (context.HasVariable(Symbol))
            {
                ISymbolValue sValue = context.GetVariable(Symbol);
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
                        context.SetVariable(Symbol, newValue);
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
