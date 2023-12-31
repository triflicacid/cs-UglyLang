﻿using UglyLang.Source.Values;

namespace UglyLang.Source.AST.Keyword
{
    /// <summary>
    /// Node for the LET keyword - create a new variable
    /// </summary>
    public class LetKeywordNode : KeywordNode
    {
        public readonly string Name;
        public readonly ExprNode Value;

        public LetKeywordNode(string name, ExprNode value)
        {
            Name = name;
            Value = value;
        }

        public override Signal Action(Context context)
        {
            Value? evaldValue = Value.Evaluate(context);
            if (evaldValue == null) // Propagate error?
                return Signal.ERROR;

            if (context.CanCreateSymbol(Name))
            {
                context.CreateSymbol(new(Name, evaldValue)
                {
                    LineNumber = LineNumber,
                    ColumnNumber = ColumnNumber,
                });
            }
            else
            {
                var variable = context.GetSymbol(Name);
                if (variable.IsReadonly)
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Name, string.Format("symbol '{0}' is read-only and cannot be shadowed", Name))
                    {
                        AppendString = string.Format("Previously defined at {0}:{1}", variable.GetLineNumber() + 1, variable.GetColumnNumber() + 1),
                        AdditionalSource = ((ILocatable)variable).GetLocation()
                    };
                    return Signal.ERROR;
                }
                else
                {
                    variable.SetValue(evaldValue);
                }
            }

            return Signal.NONE;
        }
    }
}
