using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source;
using UglyLang.Source.Functions;
using UglyLang.Source.Values;

namespace UglyLang.Source.AST
{
    /// <summary>
    /// A node which contains a reference to a symbol.
    /// </summary>
    public class SymbolNode : ASTNode
    {
        public readonly string Symbol;
        public readonly List<string> Components;

        /// If not null then the symbol is a reference to a function (or should be)
        public List<ExprNode>? CallArguments;

        public SymbolNode(string symbol)
        {
            Type = ASTNodeType.SYMBOL;
            Symbol = symbol;
            Components = symbol.Split('.').ToList();
            CallArguments = null;
        }

        public override Value? Evaluate(Context context)
        {
            if (context.HasVariable(Components[0]))
            {
                ISymbolValue variable = context.GetVariable(Components[0]);

                // Lookup all components
                for (int i = 1; i < Components.Count; i++)
                {
                    if (variable is Value varValue)
                    {
                        if (varValue.HasProperty(Components[i]))
                        {
                            variable = varValue.GetProperty(Components[i]).GetValue();
                        }
                        else
                        {
                            context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("cannot get property {0} of type {1}", Components[i], varValue.Type));
                            return null;
                        }
                    }
                    else
                    {
                        context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("cannot get property {0}", Components[i]));
                        return null;
                    }
                }

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

                    // Call function
                    Value? returnedValue = func.Call(context, arguments);

                    if (returnedValue == null || context.Error != null)
                    {
                        // Propagate error
                        if (context.Error != null)
                        {
                            context.Error.LineNumber = LineNumber;
                            context.Error.ColumnNumber = ColumnNumber;
                        }

                        return null;
                    }
                    else
                    {
                        value = returnedValue;
                    }

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
    }
}
