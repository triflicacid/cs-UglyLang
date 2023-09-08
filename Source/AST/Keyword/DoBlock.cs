namespace UglyLang.Source.AST.Keyword
{
    /// <summary>
    /// A block which creates a new scope
    /// </summary>
    public class DoBlockKeywordNode : KeywordNode
    {
        public ASTStructure? Body;

        public override Signal Action(Context context)
        {
            if (Body == null)
                throw new NullReferenceException();

            context.PushStackContext(LineNumber, ColumnNumber, StackContextType.DoBlock, this, "");

            Signal s = Body.Evaluate(context);
            if (s == Signal.ERROR)
                return s;

            context.PopStackContext();

            return s == Signal.EXIT_PROG ? s : Signal.NONE;
        }
    }
}
