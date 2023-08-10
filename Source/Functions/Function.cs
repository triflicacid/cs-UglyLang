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
        public Signal Call(Context context, List<Value> arguments);
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

        public Signal Call(Context context, List<Value> arguments)
        {
            List<Value> newArguments = new(arguments);
            newArguments.Insert(0, Context);
            return Func.Call(context, newArguments);
        }
    }

    public abstract class Function : ICallable, ISymbolValue
    {
        public readonly List<UnresolvedType[]> ArgumentTypes;
        public readonly UnresolvedType ReturnType;

        public Function(List<UnresolvedType[]> argumentTypes, UnresolvedType returnType)
        {
            ArgumentTypes = argumentTypes;
            ReturnType = returnType;
        }

        public Signal Call(Context context, List<Value> arguments)
        {
            List<Types.Type> receivedArgumentTypes = arguments.Select(a => a.Type).ToList();
            TypeParameterCollection typeParameters = new();
            List<Types.Type[]> resolvedArgumentTypes = new();

            // Check that arguments match up to expected
            bool match = false;
            int index = 0;
            foreach (UnresolvedType[] typeArray in ArgumentTypes)
            {
                if (typeArray.Length == receivedArgumentTypes.Count)
                {
                    Types.Type[] resolvedTypeArray = new Types.Type[typeArray.Length];
                    match = true;
                    for (int i = 0; i < typeArray.Length && match; i++)
                    {
                        // Attemt to resolve into a type
                        Types.Type? aType = typeArray[i].Resolve(context);
                        if (aType == null)
                        {
                            context.Error = new(0, 0, Error.Types.Type, string.Format("failed to resolve '{0}' to a type", typeArray[i].Value));
                            return Signal.ERROR;
                        }

                        resolvedTypeArray[i] = aType;
                        match = aType.DoesMatch(receivedArgumentTypes[i]);
                        if (match)
                        {
                            if (aType.IsParameterised())
                            {
                                TypeParameterCollection result = aType.MatchParametersAgainst(receivedArgumentTypes[i]);

                                // Results MUST match up with typeParameters
                                foreach (string p in result.GetParamerNames())
                                {
                                    Types.Type pType = result.GetParameter(p);

                                    if (typeParameters.HasParameter(p))
                                    {
                                        Types.Type oType = typeParameters.GetParameter(p);
                                        if (!oType.Equals(pType)) // BAD
                                        {
                                            context.Error = new(0, 0, Error.Types.Type, string.Format("type parameter {0} in argument {1}: expected {2}, got {3}", p, i + 1, oType, pType));
                                            return Signal.ERROR;
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
                                var casted = arguments[i].To(aType);
                                if (casted == null)
                                {
                                    context.Error = new(0, 0, Error.Types.Cast, string.Format("cannot cast {0} to {1}", arguments[i].Type, aType));
                                    return Signal.ERROR;
                                }
                                else
                                {
                                    arguments[i] = casted;
                                }
                            }
                        }
                    }

                    resolvedArgumentTypes.Add(resolvedTypeArray);

                    if (!match) break;
                }
                if (match) break;
                index++;
            }

            if (!match)
            {
                string error = "cannot match argument types against a signature.";
                error += Environment.NewLine + "  Received: <" + string.Join(", ", receivedArgumentTypes.Select(a => a.ToString()).ToArray()) + ">";
                error += Environment.NewLine + "  Expected: " + string.Join(" | ", resolvedArgumentTypes.Select(a => "<" + string.Join(", ", a.Select(b => b.ToString())) + ">"));

                context.Error = new(0, 0, Error.Types.Type, error);
                return Signal.ERROR;
            }

            // Register type parameters to the stack
            context.MergeTypeParams(typeParameters);

            // Set type parameters as variables, so they can be referenced as types
            foreach (string p in typeParameters.GetParamerNames())
            {
                context.CreateVariable(p, new TypeValue(typeParameters.GetParameter(p)));
            }

            // Invoke the respective overload
            return CallOverload(context, index, arguments, typeParameters);
        }

        /// <summary>
        /// Call the function. Note that the given argument list matches with ONE ArgumentTypes member (this is checked in FuncValue. The stack frames and argument evaluation are handled in SymbolNode). Return a Signal indicating the status. The return result is available via context.GetFunctionReturnValue().
        /// </summary>
        protected abstract Signal CallOverload(Context context, int overloadIndex, List<Value> arguments, TypeParameterCollection typeParameters);
    }
}
