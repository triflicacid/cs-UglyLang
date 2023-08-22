using UglyLang.Source.Values;

namespace UglyLang.Source.AST.Keyword
{
    /// <summary>
    /// Keyword to prompt for an input and place it into a variable
    /// </summary>
    public class InputKeywordNode : KeywordNode
    {
        public readonly AbstractSymbolNode Symbol;

        public InputKeywordNode(AbstractSymbolNode symbol)
        {
            Symbol = symbol;
        }

        public override Signal Action(Context context)
        {
            // Prompt user for input
            string raw = Console.ReadLine() ?? "";

            return Symbol.SetValue(context, new StringValue(raw), true)
                ? Signal.NONE
                : Signal.ERROR;
        }
    }
}
