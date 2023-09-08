using UglyLang.Source.Values;

namespace UglyLang.Source.AST.Keyword
{
    /// <summary>
    /// Node for the CONST keyword - create a new constant
    /// </summary>
    public class ConstKeywordNode : KeywordNode
    {
        public readonly string Name;
        public readonly ExprNode Value;

        public ConstKeywordNode(string name, ExprNode value)
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
                    IsReadonly = true
                });
            }
            else
            {
                var variable = context.GetSymbol(Name);
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Name, string.Format("symbol '{0}' already exists", Name))
                {
                    AppendString = string.Format("Previously defined at {0}:{1}", variable.GetLineNumber() + 1, variable.GetColumnNumber() + 1),
                    AdditionalSource = ((ILocatable)variable).GetLocation()
                };
                
                return Signal.ERROR;
            }

            return Signal.NONE;
        }
    }
}
