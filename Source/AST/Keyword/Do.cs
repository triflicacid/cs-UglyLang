﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source;
using UglyLang.Source.Values;

namespace UglyLang.Source.AST.Keyword
{
    /// <summary>
    /// Evaluate an expression and throw away the result
    /// </summary>
    public class DoKeywordNode : KeywordNode
    {
        public readonly ExprNode Expr;

        public DoKeywordNode(ExprNode expr) : base("DO")
        {
            Expr = expr;
        }

        public override Signal Action(Context context)
        {
            Value value = Expr.Evaluate(context);
            if (context.Error != null) // Propagate error?
                return Signal.ERROR;
            
            return Signal.NONE;
        }
    }
}