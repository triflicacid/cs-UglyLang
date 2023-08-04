using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.AST;
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

        public UserFunction(Values.ValueType? returnType) : base(new(), returnType) { }

        /// <summary>
        /// Add a function overload
        /// </summary>
        public void AddOverload(List<(string, Values.ValueType)> arguments, ASTStructure body)
        {
            Bodies.Add(body);
            ArgumentNames.Add(arguments.Select(p => p.Item1).ToArray());
            ArgumentTypes.Add(arguments.Select(p => p.Item2).ToArray());
        }

        public override Value? Call(Context context, int overloadIndex, List<Value> arguments)
        {
            if (arguments.Count != ArgumentNames[overloadIndex].Length)
            {
                throw new NotSupportedException(); // Shouldn't happen. Matching against the correct overload should've been handled. See FuncValue.
            }

            // Get appropriate code to execute
            ASTStructure body = Bodies[overloadIndex];

            // Create variables for the parameters
            for (int i = 0; i < arguments.Count; i++)
            {
                context.CreateVariable(ArgumentNames[overloadIndex][i], arguments[i]);
            }

            // Evaluate function
            body.Evaluate(context);

            // Get return value
            Value? returnValue = context.GetFunctionReturnValue();
            if (returnValue == null)
            {
                if (ReturnType == null)
                {
                    return new EmptyValue();
                }
                else
                {
                    context.Error = new(0, 0, Error.Types.Type, string.Format("expected return type of {0}, got (none)", ReturnType));
                    return null;
                }
            }
            else if (ReturnType == null)
            {
                context.Error = new(0, 0, Error.Types.Type, string.Format("expected return type of (none), got {0}", returnValue.Type));
                return null;
            }

            if (!Value.Match((Values.ValueType)ReturnType, returnValue.Type))
            {
                context.Error = new(0, 0, Error.Types.Type, string.Format("cannot match returned type {0} with expected {1}", returnValue.Type, ReturnType));
                return null;
            }

            return returnValue.To((Values.ValueType)ReturnType);
        }
    }
}
