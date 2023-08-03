using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source;
using UglyLang.Source.Values;

namespace UglyLang.Source.AST
{
    /// <summary>
    /// A node which contains a reference to a symbol.
    /// </summary>
    public class SymbolNode : ASTNode
    {
        public string Symbol;

        /// If not null then the symbol is a reference to a function (or should be)
        public List<ExprNode>? CallArguments;

        public SymbolNode(string symbol)
        {
            Type = ASTNodeType.SYMBOL;
            Symbol = symbol;
            CallArguments = null;
        }

        public override Value Evaluate(Context context)
        {
            if (context.HasVariable(Symbol))
            {
                Value variable = context.GetVariable(Symbol);
                Value value;

                if (variable is FuncValue func)
                {
                    // Push new stack context
                    context.PushStackContext(LineNumber, ColumnNumber, Context.StackContext.Types.Function, Symbol);

                    // Evaluate arguments
                    List<Value> arguments = new();
                    if (CallArguments != null)
                    {
                        foreach (ExprNode expr in CallArguments)
                        {
                            Value arg = expr.Evaluate(context);
                            if (context.Error != null)
                            {
                                return new EmptyValue(); // Propagate error
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

                        return new EmptyValue();
                    }
                    else
                    {
                        value = returnedValue;
                    }

                    // Pop stack context
                    context.PopStackContext();
                }
                else
                {
                    value = variable;
                }

                return value;
            }
            else
            {
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Name, Symbol);
                return new EmptyValue();
            }
        }
    }
}
