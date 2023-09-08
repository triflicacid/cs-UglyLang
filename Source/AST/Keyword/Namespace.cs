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

        public override Signal Action(Context context)
        {
            if (Body == null)
                throw new NullReferenceException();

            if (!context.CanCreateSymbol(Name))
            {
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Name, string.Format("{0} is already defined", Name));
                return Signal.ERROR;
            }

            // Execute in a new scope, then export to a namespace
            NamespaceValue ns = new();
            context.PushStack(ns);
            Signal s = Body.Evaluate(context);
            if (s == Signal.ERROR)
                return s;
            context.PopStack();
            context.CreateSymbol(new(Name, ns));

            return Signal.NONE;
        }
    }
}
