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

        public UserFunction(Values.ValueType returnType) : base(new(), returnType) { }

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

            return new EmptyValue().To(ReturnType);
        }
    }
}
