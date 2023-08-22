using UglyLang.Source.Types;

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

            // Evaluate and collection data definitions
            UserTypeDataContainer data = new(Name);
            context.PushStack(data);
            Signal s = Body.Evaluate(context);
            if (s == Signal.ERROR) return s;
            context.PopStack();

            // Create user type
            UserType type = new(data);
            context.CreateSymbol(Name, type);

            return Signal.NONE;
        }
    }
}
