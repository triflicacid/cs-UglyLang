using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Functions;

namespace UglyLang.Source.Values
{
    /// <summary>
    /// Value representing a function
    /// </summary>
    public class FuncValue : Value
    {
        public Function Func;

        public FuncValue(Function func)
        {
            Type = ValueType.FUNCTION;
            Func = func;
        }

        public Value? Call(Context context, List<Value> arguments)
        {
            List<ValueType> receivedArgumentTypes = arguments.Select(a => a.Type).ToList();

            // Check that arguments match up to expected
            bool match = false;
            int index = 0;
            foreach (ValueType[] typeArray in Func.ArgumentTypes)
            {
                if (typeArray.Length == receivedArgumentTypes.Count)
                {
                    match = true;
                    for (int i = 0; i < typeArray.Length && match; i++)
                    {
                        match = Match(receivedArgumentTypes[i], typeArray[i]);
                        if (match && typeArray[i] != ValueType.ANY)
                        {
                            arguments[i] = arguments[i].To(typeArray[i]);
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
                error += Environment.NewLine + "  Expected: " + string.Join(" | ", Func.ArgumentTypes.Select(a => "<" + string.Join(", ", a.Select(b => b.ToString())) + ">"));

                context.Error = new(0, 0, Error.Types.Type, error);
                return null;
            }

            Value? value = Func.Call(context, index, arguments);

            return value;
        }

        public override bool IsTruthy()
        {
            return true;
        }

        public static Function From(Value value)
        {
            throw new NotSupportedException();
        }

        public override Value To(ValueType type)
        {
            throw new NotSupportedException();
        }
    }
}
