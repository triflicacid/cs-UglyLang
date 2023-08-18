using UglyLang.Source.Values;

namespace UglyLang.Source.AST.Keyword
{
    public class NamespaceKeywordNode : KeywordNode
    {
        public readonly string Name;
        public ASTStructure? Body;

        public NamespaceKeywordNode(string name)
        {
            Name = name;
            Body = null;
        }

        public override Signal Action(Context context, ISymbolContainer container)
        {
            if (Body == null)
                throw new NullReferenceException();

            if (container.HasSymbol(Name))
            {
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Name, string.Format("{0} is already defined", Name));
                return Signal.ERROR;
            }

            // Create and execute the body of the namespace. Passing the namespace as the 2nd argument means that any symbol created on evaluation will be created in the namespace.
            NamespaceValue ns = new();
            Body.Evaluate(context, ns);
            container.CreateSymbol(Name, ns);

            return Signal.NONE;
        }
    }
}
