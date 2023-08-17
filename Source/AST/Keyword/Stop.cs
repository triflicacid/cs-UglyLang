namespace UglyLang.Source.AST.Keyword
{
    /// <summary>
    /// Halt the program's execution
    /// </summary>
    public class StopKeywordNode : KeywordNode
    {
        public override Signal Action(Context context, ISymbolContainer container)
        {
            return Signal.EXIT_PROG;
        }
    }
}
