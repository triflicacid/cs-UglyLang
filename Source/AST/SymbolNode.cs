using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source;

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
            if (CallArguments == null)
            {
                if (context.HasVariable(Symbol))
                {
                    Value value = context.GetVariable(Symbol);
                    return CastType == null ? value : value.To((ValueType) CastType);
                }
                else
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Name, Symbol);
                    return new EmptyValue();
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
