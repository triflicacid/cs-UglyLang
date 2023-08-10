using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.AST;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions
{
    /// <summary>
    /// A user-defined function
    /// </summary>
    public class UserFunction : Function
    {
        private readonly List<ASTStructure> Bodies = new();
        private readonly List<string[]> ArgumentNames = new();

        public UserFunction(UnresolvedType returnType) : base(new(), returnType) { }

        /// <summary>
        /// Add a function overload
        /// </summary>
        public void AddOverload(List<(string, UnresolvedType)> arguments, ASTStructure body)
        {
            Bodies.Add(body);
            ArgumentNames.Add(arguments.Select(p => p.Item1).ToArray());
            ArgumentTypes.Add(arguments.Select(p => p.Item2).ToArray());
        }

        protected override Signal CallOverload(Context context, int overloadIndex, List<Value> arguments, TypeParameterCollection typeParameters)
        {
            if (arguments.Count != ArgumentNames[overloadIndex].Length)
            {
                throw new NotSupportedException(); // Shouldn't happen. Matching against the correct overload should've been handled. See FuncValue.
            }

            // Resolve the return value
            Types.Type? returnType = ReturnType.Resolve(context);
            if (returnType == null)
            {
                context.Error = new(0, 0, Error.Types.Type, string.Format("failed to resolve '{0}' to a type", ReturnType.Value));
                return Signal.ERROR;
            }

            // Get appropriate code to execute
            ASTStructure body = Bodies[overloadIndex];

            // Create variables for the parameters
            for (int i = 0; i < arguments.Count; i++)
            {
                context.CreateVariable(ArgumentNames[overloadIndex][i], arguments[i]);
            }

            // Evaluate function
            Signal s = body.Evaluate(context);
            if (s == Signal.ERROR) return Signal.ERROR;

            // Get return value
            Value returnValue = context.GetFunctionReturnValue() ?? new EmptyValue();

            // Check if return types match
            if (returnType.IsParameterised())
            {
                TypeParameterCollection result = returnType.MatchParametersAgainst(returnValue.Type);

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
            else if (!returnType.DoesMatch(returnValue.Type))
            {
                context.Error = new(0, 0, Error.Types.Type, string.Format("cannot match returned type {0} with expected {1}", returnValue.Type, returnType));
                return Signal.ERROR;
            }

            // Cast return value to the desired type
            Value? casted = returnValue.To(returnType);
            if (casted == null)
            {
                context.Error = new(0, 0, Error.Types.Cast, string.Format("cannot cast {0} to {1}", returnValue.Type, returnType));
                return Signal.ERROR;
            }

            // Set return value
            context.SetFunctionReturnValue(casted);

            return Signal.NONE;
        }
    }
}
