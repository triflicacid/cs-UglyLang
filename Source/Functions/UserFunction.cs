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

        public UserFunction(Types.Type returnType) : base(new(), returnType) { }

        /// <summary>
        /// Add a function overload
        /// </summary>
        public void AddOverload(List<(string, Types.Type)> arguments, ASTStructure body)
        {
            Bodies.Add(body);
            ArgumentNames.Add(arguments.Select(p => p.Item1).ToArray());
            ArgumentTypes.Add(arguments.Select(p => p.Item2).ToArray());
        }

        protected override Value? CallOverload(Context context, int overloadIndex, List<Value> arguments)
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
            Signal s = body.Evaluate(context);
            if (s == Signal.ERROR) return null;

            // Get return value
            Value? returnValue = context.GetFunctionReturnValue();
            if (returnValue == null)
            {
                if (context.Error != null) throw new NullReferenceException(); // If the function returned null, we expect there to have been an error.
                return null;
            }

            if (!ReturnType.DoesMatch(returnValue.Type))
            {
                context.Error = new(0, 0, Error.Types.Type, string.Format("cannot match returned type {0} with expected {1}", returnValue.Type, ReturnType));
                return null;
            }

            Value? casted = returnValue.To(ReturnType);
            if (casted == null)
            {
                context.Error = new(0, 0, Error.Types.Cast, string.Format("cannot cast {0} to {1}", returnValue.Type, ReturnType));
                return null;
            }

            return casted;
        }
    }
}
