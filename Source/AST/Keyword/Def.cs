using UglyLang.Source.Functions;
using UglyLang.Source.Types;

namespace UglyLang.Source.AST.Keyword
{
    /// <summary>
    /// Define a function
    /// </summary>
    public class DefKeywordNode : KeywordNode
    {
        public readonly string Name;
        public readonly List<(string, UnresolvedType)> Arguments;
        public readonly UnresolvedType ReturnType; // If NULL, returns nothing
        public ASTStructure? Body;
        public readonly Dictionary<string, List<UnresolvedType>> TypeParamConstraints;

        public DefKeywordNode(string name, List<(string, UnresolvedType)> arguments, UnresolvedType returnType, Dictionary<string, List<UnresolvedType>>? constraints = null)
        {
            Name = name;
            Arguments = arguments;
            Body = null;
            ReturnType = returnType;
            TypeParamConstraints = constraints ?? new();
        }

        public override Signal Action(Context context)
        {
            if (Body == null)
                throw new NullReferenceException();

            // Check if the function is already defined
            Function func;
            if (context.HasSymbol(Name))
            {
                ISymbolValue variable = context.GetSymbol(Name);
                if (variable is Function funcValue)
                {
                    func = funcValue;
                }
                else
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Name, string.Format("'{0}' is already defined and is not a function", Name));
                    return Signal.ERROR;
                }
            }
            else
            {
                func = new Function();
                context.CreateSymbol(Name, func);
            }


            // Resolve the arguments
            List<(string, Types.Type)> resolvedArguments = new();
            foreach ((string argName, UnresolvedType argType) in Arguments)
            {
                Types.Type? type = argType.Resolve(context);
                if (type == null)
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("failed to resolve type {0}", argType.Value.GetSymbolString()));
                    return Signal.ERROR;
                }

                resolvedArguments.Add(new(argName, type));
            }

            // Resolve the constraints
            Dictionary<string, Types.Type[]> resolvedConstraints = new();
            foreach (var entry in TypeParamConstraints)
            {
                Types.Type[] types = new Types.Type[entry.Value.Count];
                for (int i = 0; i < types.Length; i++)
                {
                    Types.Type? resolved = entry.Value[i].Resolve(context);
                    if (resolved == null)
                    {
                        context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("failed to resolve type {0}", entry.Value[i].Value.GetSymbolString()));
                        return Signal.ERROR;
                    }

                    if (resolved.IsParameterised())
                    {
                        context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("type constraints cannot be parameterised (found '{0}')", resolved));
                        return Signal.ERROR;
                    }

                    types[i] = resolved;
                }

                resolvedConstraints.Add(entry.Key, types);
            }

            // Resolve the return type
            Types.Type? resolvedReturnType = ReturnType.Resolve(context);
            if (resolvedReturnType == null)
            {
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("failed to resolve type {0}", ReturnType.Value.GetSymbolString()));
                return Signal.ERROR;
            }

            // Create the overload and attempt to register it with the function
            UserFunctionOverload overload = new(resolvedArguments, Body, resolvedReturnType, resolvedConstraints);
            if (!func.RegisterOverload(overload))
            {
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("an overload matching this signature has already been defined\n<{0}> -> {1}", string.Join(",", resolvedArguments.Select(p => p.Item2)), resolvedReturnType));
                return Signal.ERROR;
            }

            return Signal.NONE;
        }
    }
}
