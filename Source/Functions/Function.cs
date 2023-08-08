using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions
{
    public interface ICallable
    {
        /// <summary>
        /// Call the given function with said arguments. Redirect call to CallOverload once the correct overload has been found.
        /// </summary>
        public Value? Call(Context context, List<Value> arguments);
    }

    /// <summary>
    /// A wrapper around a function with a given value as the "context". This will be automatically prepended to the argument list on call.
    /// </summary>
    public class FunctionContext : ICallable, ISymbolValue
    {
        public readonly Function Func;
        public readonly Value Context;

        public FunctionContext(Function func, Value context)
        {
            Func = func;
            Context = context;
        }

        public Value? Call(Context context, List<Value> arguments)
        {
            List<Value> newArguments = new(arguments);
            newArguments.Insert(0, Context);
            return Func.Call(context, newArguments);
        }
    }

    public abstract class Function : ICallable, ISymbolValue
    {
        public readonly List<Types.Type[]> ArgumentTypes;
        public readonly Types.Type ReturnType;

        public Function(List<Types.Type[]> argumentTypes, Types.Type returnType)
        {
            ArgumentTypes = argumentTypes;
            ReturnType = returnType;
        }

        public Value? Call(Context context, List<Value> arguments)
        {
            List<Types.Type> receivedArgumentTypes = arguments.Select(a => a.Type).ToList();
            TypeParameterCollection typeParameters = new();

            // Check that arguments match up to expected
            bool match = false;
            int index = 0;
            foreach (Types.Type[] typeArray in ArgumentTypes)
            {
                if (typeArray.Length == receivedArgumentTypes.Count)
                {
                    match = true;
                    for (int i = 0; i < typeArray.Length && match; i++)
                    {
                        match = typeArray[i].DoesMatch(receivedArgumentTypes[i]);
                        if (match)
                        {
                            if (typeArray[i].IsParameterised())
                            {
                                TypeParameterCollection result = typeArray[i].MatchParametersAgainst(receivedArgumentTypes[i]);

                                // Results MUST match up with typeParameters
                                foreach (string p in result.GetParamerNames())
                                {
                                    Types.Type pType = result.GetParameter(p);

                                    if (typeParameters.HasParameter(p))
                                    {
                                        Types.Type type = typeParameters.GetParameter(p);
                                        if (!type.Equals(pType)) // BAD
                                        {
                                            context.Error = new(0, 0, Error.Types.Type, string.Format("type parameter {0} in argument {1}: expected {2}, got {3}", p, i + 1, type, pType));
                                            return null;
                                        }
                                    }
                                    else
                                    {
                                        // New parameter
                                        typeParameters.SetParameter(p, pType);
                                    }
                                }
                            }
                            else
                            {
                                var casted = arguments[i].To(typeArray[i]);
                                if (casted == null)
                                {
                                    context.Error = new(0, 0, Error.Types.Cast, string.Format("cannot cast {0} to {1}", arguments[i].Type, typeArray[i]));
                                    return null;
                                }
                                else
                                {
                                    arguments[i] = casted;
                                }
                            }
                        }
                    }

                    if (!match) break;
                }
                if (match) break;
                index++;
            }

            if (!match)
            {
                string error = "cannot match argument types against a signature.";
                error += Environment.NewLine + "  Received: <" + string.Join(", ", receivedArgumentTypes.Select(a => a.ToString()).ToArray()) + ">";
                error += Environment.NewLine + "  Expected: " + string.Join(" | ", ArgumentTypes.Select(a => "<" + string.Join(", ", a.Select(b => b.ToString())) + ">"));

                context.Error = new(0, 0, Error.Types.Type, error);
                return null;
            }

            // Set type parameters as variables
            foreach (string p in typeParameters.GetParamerNames())
            {
                context.CreateVariable(p, new TypeValue(typeParameters.GetParameter(p)));
            }

            Value? value = CallOverload(context, index, arguments);

            return value;
        }

        /// <summary>
        /// Call the function. Note that the given argument list matches with ONE ArgumentTypes member (this is checked in FuncValue. The stack frames and argument evaluation are handled in SymbolNode).
        /// </summary>
        protected abstract Value? CallOverload(Context context, int overloadIndex, List<Value> arguments);
    }
}
