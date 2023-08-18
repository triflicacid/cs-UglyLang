namespace UglyLang.Source.AST.Keyword
{
    /// <summary>
    /// A block which creates a new scope
    /// </summary>
    public class DoBlockKeywordNode : KeywordNode
    {
        public ASTStructure? Body;

        public override Signal Action(Context context, ISymbolContainer container)
        {
            if (Body == null)
                throw new NullReferenceException();

            context.PushStackContext(LineNumber, ColumnNumber, StackContextType.DoBlock, "");

            Signal s = Body.Evaluate(context, container);
            if (s == Signal.ERROR)
                return s;

            context.PopStackContext();

            return s == Signal.EXIT_PROG ? s : Signal.NONE;
        }
    }
}
