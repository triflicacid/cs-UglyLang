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
        public new readonly Types.Type CastType;

        public CastKeywordNode(string symbol, Types.Type type) : base("CAST")
        {
            Symbol = symbol;
            CastType = type;
        }

        public override Signal Action(Context context)
        {
            if (context.HasVariable(Symbol))
            {
                ISymbolValue sValue = context.GetVariable(Symbol);
                if (sValue is Value value)
                {
                    Value? newValue = value.To(CastType);
                    if (newValue == null)
                    {
                        context.Error = new(LineNumber, ColumnNumber, Error.Types.Cast, string.Format("casting {0} to type {1}", Symbol, CastType));
                        return Signal.ERROR;
                    }
                    else
                    {
                        context.SetVariable(Symbol, newValue);
                        return Signal.NONE;
                    }
                }
                else
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("cannot cast symbol '{0}'", Symbol));
                    return Signal.ERROR;
                }
            }
            else
            {
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Name, Symbol);
                return Signal.ERROR;
            }
        }
    }
}
