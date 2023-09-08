using UglyLang.Source.AST;
using UglyLang.Source.Values;

namespace UglyLang.Source.Types
{
    /// <summary>
    /// An overload of a function.
    /// </summary>
    public abstract class FunctionOverload : ILocatable
    {
        public readonly Type[] ArgumentTypes;
        public readonly Type ReturnType;
        public readonly Dictionary<string, Type[]> Constraints;
        public int LineNumber = 0;
        public int ColumnNumber = 0;

        public FunctionOverload(Type[] argumentTypes, Type returnType, Dictionary<string, Type[]>? constraints = null)
        {
            ArgumentTypes = argumentTypes;
            ReturnType = returnType;
            Constraints = constraints ?? new();
        }

        /// <summary>
        /// Check if the given arguments match against this overload. Populate the type parameter collection with the resolved type parameters. The context.Error field may be set, so keep an eye out for that.
        /// </summary>
        public bool IsMatch(Context context, List<Type> arguments, TypeParameterCollection typeParameters)
        {
            if (arguments.Count != ArgumentTypes.Length)
                return false;

            for (int i = 0; i < ArgumentTypes.Length; i++)
            {
                if (ArgumentTypes[i].DoesMatch(arguments[i]))
                {
                    // Is the type paramerised? If so, check
                    if (ArgumentTypes[i].IsParameterised())
                    {
                        TypeParameterCollection result = ArgumentTypes[i].MatchParametersAgainst(arguments[i]);

                        foreach (string p in result.GetParamerNames())
                        {
                            Type pType = result.GetParameter(p);

                            // Does the parameter match up with the constraints
                            if (Constraints.TryGetValue(p, out Type[]? constraints))
                            {
                                bool found = false;
                                foreach (Type constraint in constraints)
                                {
                                    if (pType.Equals(constraint))
                                    {
                                        found = true;
                                        break;
                                    }
                                }

                                if (!found)
                                    return false;
                            }

                            // Does the result match with the existing type parameter type?
                            if (typeParameters.HasParameter(p))
                            {
                                Type oType = typeParameters.GetParameter(p);
                                if (!oType.Equals(pType)) // BAD
                                {
                                    context.Error = new(0, 0, Error.Types.Type, string.Format("type parameter {0} in argument {1}: expected {2}, got {3}", p, i + 1, oType, pType));
                                    return false;
                                }
                            }
                            else
                            {
                                // New parameter
                                typeParameters.SetParameter(p, pType);
                            }
                        }
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Call this overload.
        /// </summary>
        public abstract Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNumber, int colNumber);

        public int GetLineNumber()
        {
            return LineNumber;
        }

        public int GetColumnNumber()
        {
            return ColumnNumber;
        }
    }

    public class UserFunctionOverload : FunctionOverload
    {
        public readonly string[] ParameterNames;
        public readonly ASTStructure Body;

        public UserFunctionOverload(List<(string, Type)> arguments, ASTStructure body, Type returnType, Dictionary<string, Type[]>? constraints = null)
        : base(arguments.Select(p => p.Item2).ToArray(), returnType, constraints)
        {
            ParameterNames = arguments.Select(p => p.Item1).ToArray();
            Body = body;
        }

        public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNumber, int colNumber)
        {
            // Create variables for the parameters
            for (int i = 0; i < arguments.Count; i++)
            {
                context.CreateSymbol(new(ParameterNames[i], arguments[i]));
            }

            // Evaluate the function's body
            Signal s = Body.Evaluate(context);

            return s == Signal.ERROR || s == Signal.EXIT_PROG ? s : Signal.NONE;
        }
    }
}
