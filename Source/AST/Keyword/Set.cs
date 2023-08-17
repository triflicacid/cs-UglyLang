﻿using UglyLang.Source.Values;

namespace UglyLang.Source.AST.Keyword
{
    /// <summary>
    /// Node for the LET keyword - create a new variable
    /// </summary>
    public class SetKeywordNode : KeywordNode
    {
        public readonly AbstractSymbolNode Symbol;
        public readonly ExprNode Expr;

        public SetKeywordNode(AbstractSymbolNode symbol, ExprNode expr)
        {
            Symbol = symbol;
            Expr = expr;
        }

        public override Signal Action(Context context, ISymbolContainer container)
        {
            // Evaluate the expression.
            Value? evaldValue = Expr.Evaluate(context, container);

            // Attempt to set the symbol to this new value
            return evaldValue != null && Symbol.SetValue(context, container, evaldValue) ? Signal.NONE : Signal.ERROR;
        }
    }
}
