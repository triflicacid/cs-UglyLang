using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UglyLang.Source.Types;
using UglyLang.Source.Values;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

    /// <summary>
    /// A function is simply a collection of overloads under one common reference.
    /// </summary>
    public class Function : ICallable, ISymbolValue
    {
        protected readonly List<FunctionOverload> Overloads = new();

        /// <summary>
        /// Does this function contain an overload which matches the given signature
        /// </summary>
        public bool DoesOverloadExist(Types.Type[] argumentTypes, Types.Type returnType)
        {
            // TODO
            /*foreach (FunctionOverload overload in Overloads)
            {

            }*/

            return false;
        }

        public bool RegisterOverload(FunctionOverload overload)
        {
            if (DoesOverloadExist(overload.ArgumentTypes, overload.ReturnType))
                return false;

            Overloads.Add(overload);
            return true;
        }

        /// <summary>
        /// Call said function with the given arguments.
        /// </summary>
        public Signal Call(Context context, List<Value> arguments)
        {
            List<Types.Type> receivedArgumentTypes = arguments.Select(a => a.Type).ToList();
            
            FunctionOverload? chosenOverload = null;
            TypeParameterCollection typeParameters = new();

            foreach (FunctionOverload overload in Overloads)
            {
                if (overload.IsMatch(context, receivedArgumentTypes, typeParameters))
                {
                    chosenOverload = overload;
                    break;
                }
                else
                {
                    typeParameters.Clear();

                    // Check if error
                    if (context.Error != null)
                        return Signal.ERROR;
                }
            }

            // Is there a match?
            if (chosenOverload == null)
            {
                string error = "cannot match argument types against a signature.";
                error += Environment.NewLine + "  Received: <" + string.Join(", ", receivedArgumentTypes.Select(a => a.ToString()).ToArray()) + ">";
                error += Environment.NewLine + "  Expected: " + string.Join(" | ", Overloads.Select(o => "<" + string.Join(", ", o.ArgumentTypes.Select(b => b.ToString())) + ">"));

                context.Error = new(0, 0, Error.Types.Type, error);
                return Signal.ERROR;
            }

            // Resolve argument types of parameterisations
            Types.Type[] argumentTypes = new Types.Type[chosenOverload.ArgumentTypes.Length];
            for (int i = 0; i < chosenOverload.ArgumentTypes.Length; i++)
            {
                if (chosenOverload.ArgumentTypes[i].IsParameterised())
                {
                    Types.Type resolved = chosenOverload.ArgumentTypes[i].ResolveParametersAgainst(typeParameters);

                    if (resolved.IsParameterised())
                    {
                        context.Error = new(0, 0, Error.Types.Type, string.Format("cannot resolve parameterised type '{0}' (partially resolved to {1})", chosenOverload.ArgumentTypes[i], resolved));
                        return Signal.ERROR;
                    }

                    argumentTypes[i] = resolved;
                }
                else
                {
                    argumentTypes[i] = chosenOverload.ArgumentTypes[i];
                }
            }


            // Cast arguments - the types match, but may not be equal
            for (int i = 0; i < arguments.Count; i++)
            {
                if (!arguments[i].Type.Equals(argumentTypes[i]))
                {
                    Value? casted = arguments[i].To(argumentTypes[i]);
                    if (casted == null)
                    {
                        context.Error = new(0, 0, Error.Types.Cast, string.Format("cannot cast {0} to {1}", arguments[i].Type, argumentTypes[i]));
                        return Signal.ERROR;
                    }

                    arguments[i] = casted;
                }
            }

            // Register type parameters to the stack
            context.MergeTypeParams(typeParameters);

            // Set type parameters as variables, so they can be referenced as types
            foreach (string p in typeParameters.GetParamerNames())
            {
                context.CreateVariable(p, new TypeValue(typeParameters.GetParameter(p)));
            }

            // Invoke the overload
            Signal sig = chosenOverload.Call(context, arguments, typeParameters);
            if (sig == Signal.ERROR || sig == Signal.EXIT_PROG)
                return sig;

            // Check against the return type
            Value returnedValue = context.GetFunctionReturnValue() ?? new EmptyValue();
            Types.Type returnType = chosenOverload.ReturnType; // NOTE that this is the expected rteurn type, NOT the type of returnedValue

            if (returnType.IsParameterised()) // If it is parameterised, resolve it. If it is still parameterised, something went wrong.
            {
                TypeParameterCollection result = returnType.MatchParametersAgainst(returnedValue.Type);

                // Results MUST match up with typeParameters
                foreach (string p in result.GetParamerNames())
                {
                    Types.Type? pType = result.GetParameter(p);

                    // Make sure that they're equal, otherwise we have a contradiction in the type parameters
                    if (typeParameters.HasParameter(p))
                    {
                        Types.Type oType = typeParameters.GetParameter(p);
                        if (!oType.Equals(pType)) // BAD
                        {
                            context.Error = new(0, 0, Error.Types.Type, string.Format("type parameter {0}: expected {1}, got {2}", p, oType, pType));
                            return Signal.ERROR;
                        }
                    }
                    else
                    {
                        // Unknown parameter
                        context.Error = new(0, 0, Error.Types.Type, string.Format("unbound type parameter '{0}'", p));
                        return Signal.ERROR;
                    }
                }

                returnType = returnType.ResolveParametersAgainst(typeParameters);
                if (returnType.IsParameterised()) // If the return type is still paramerised, we have a problem
                {
                    context.Error = new(0, 0, Error.Types.Type, string.Format("parameterised type {0} cannot be resolved", returnType));
                    return Signal.ERROR;
                }
            }
            else if (!returnType.DoesMatch(returnedValue.Type))
            {
                context.Error = new(0, 0, Error.Types.Type, string.Format("expected return value to be {0}, got {1}", returnType, returnedValue.Type));
                return Signal.ERROR;
            }

            // Cast if not equal
            if (!returnType.Equals(returnedValue.Type))
            {
                // Cast return value to the desired type
                Value? casted = returnedValue.To(returnType);
                if (casted == null)
                {
                    context.Error = new(0, 0, Error.Types.Cast, string.Format("cannot cast {0} to {1}", returnedValue.Type, returnType));
                    return Signal.ERROR;
                }
            }

            // All is good; the return type matches.
            return Signal.NONE;
        }
    }
}
