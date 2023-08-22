using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.AST.Keyword
{
    public class TypeKeywordNode : KeywordNode
    {
        public readonly string Name;
        public ASTStructure? Body;

        public TypeKeywordNode(string name)
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

            // Divide up fields/methods and static properties
            ASTStructure statics = new(), dynamics = new();
            foreach (ASTNode node in Body)
            {
                if (node is LetKeywordNode)
                {
                    statics.AddNode(node);
                }
                else
                {
                    dynamics.AddNode(node);
                }
            }

            // Evaluate and collection data definitions
            UserTypeDataContainer data = new(Name);
            context.PushStack(data);
            Signal s = dynamics.Evaluate(context);
            if (s == Signal.ERROR) return s;
            context.PopStack();

            // Create user type
            UserType type = new(data);
            context.CreateSymbol(Name, type);

            NamespaceValue ns = new();
            context.PushStack(ns);
            s = statics.Evaluate(context);
            if (s == Signal.ERROR) return s;
            context.PopStack();
            type.Statics = ns;

            return Signal.NONE;
        }
    }
}
