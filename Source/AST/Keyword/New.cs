using UglyLang.Source.Types;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.AST.Keyword
{
    /// <summary>
    /// Represents a type's constructor
    /// </summary>
    public class NewKeywordNode : KeywordNode
    {
        public readonly List<(string, UnresolvedType)> Arguments;
        public ASTStructure? Body = null;

        public NewKeywordNode(List<(string, UnresolvedType)> arguments)
        {
            Arguments = arguments;
        }

        public override Signal Action(Context context)
        {
            if (Body == null)
                throw new NullReferenceException();

            // Resolve the arguments
            List<(string, Type)> resolvedArguments = new();
            foreach ((string argName, UnresolvedType argType) in Arguments)
            {
                Type? type = argType.Resolve(context);
                if (type == null)
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("failed to resolve type {0}", argType.Value.GetSymbolString()));
                    return Signal.ERROR;
                }

                resolvedArguments.Add(new(argName, type));
            }

            // Retrieve the type data
            UserTypeDataContainer data = (UserTypeDataContainer)context.PeekStack();

            // Does the constructor already exist?
            if (data.Constructor.DoesOverloadExist(resolvedArguments.Select(p => p.Item2).ToArray()))
            {
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Name, string.Format("duplicate definition for constructor in type {0}", data.Name));
                return Signal.ERROR;
            }
            else
            {
                UserFunctionOverload overload = new(resolvedArguments, Body, Type.EmptyT);
                data.Constructor.RegisterOverload(overload, true);

                return Signal.NONE;
            }
        }
    }
}
