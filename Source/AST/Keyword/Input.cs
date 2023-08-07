using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Values;

namespace UglyLang.Source.AST.Keyword
{
    /// <summary>
    /// Keyword to prompt for an input and place it into a variable
    /// </summary>
    public class InputKeywordNode : KeywordNode
    {
        public readonly string Symbol;

        public InputKeywordNode(string symbol) : base("INPUT")
        {
            Symbol = symbol;
        }

        public override Signal Action(Context context)
        {
            if (context.HasVariable(Symbol))
            {
                ISymbolValue oldSymbolValue = context.GetVariable(Symbol);

                // Prompt user for input
                string raw = Console.ReadLine() ?? "";
                Value rawValue = new StringValue(raw);

                if (oldSymbolValue is Value oldValue)
                {
                    // Cast to correct type
                    Value? value = rawValue.To(oldValue.Type);

                    if (value == null)
                    {
                        context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("error casting {0} to {1}", rawValue.Type, oldValue.Type));
                        return Signal.ERROR;
                    }

                    context.SetVariable(Symbol, value);
                }
                else
                {
                    context.SetVariable(Symbol, rawValue);
                }

                return Signal.NONE;
            }
            else
            {
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Name, Symbol);
                return Signal.ERROR;
            }
        }
    }
}
