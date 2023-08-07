using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions
{
    public abstract class Function : ISymbolValue
    {
        public readonly List<Types.Type[]> ArgumentTypes;
        public readonly Types.Type? ReturnType;

        public Function(List<Types.Type[]> argumentTypes, Types.Type? returnType)
        {
            ArgumentTypes = argumentTypes;
            ReturnType = returnType;
        }

        /// <summary>
        /// Call the given function with said arguments. Redirect call to CallOverload once the correct overload has been found.
        /// </summary>
        public Value? Call(Context context, List<Value> arguments)
        {
            List<Types.Type> receivedArgumentTypes = arguments.Select(a => a.Type).ToList();

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
                            arguments[i] = arguments[i].To(typeArray[i]) ?? arguments[i];
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

            Value? value = CallOverload(context, index, arguments);

            return value;
        }

        /// <summary>
        /// Call the function. Note that the given argument list matches with ONE ArgumentTypes member (this is checked in FuncValue. The stack frames and argument evaluation are handled in SymbolNode).
        /// </summary>
        protected abstract Value? CallOverload(Context context, int overloadIndex, List<Value> arguments);
    }
}
