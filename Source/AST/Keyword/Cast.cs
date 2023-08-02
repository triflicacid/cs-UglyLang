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
    /// Node for the LET keyword - create a new variable
    /// </summary>
    public class CastKeywordNode : KeywordNode
    {
        public readonly string Symbol;
        public new readonly Values.ValueType CastType;

        public CastKeywordNode(string symbol, Values.ValueType type) : base("CAST")
        {
            Symbol = symbol;
            CastType = type;
        }

        public override Signal Action(Context context)
        {
            if (context.HasVariable(Symbol))
            {
                Value value = context.GetVariable(Symbol);
                Value newValue = value.To(CastType);
                context.SetVariable(Symbol, newValue);
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
