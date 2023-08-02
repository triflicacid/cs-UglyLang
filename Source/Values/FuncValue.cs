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
    public abstract class FuncValue : Value
    {
        public FuncValue()
        {
            Type = ValueType.FUNCTION;
        }

        public abstract Value? Call(Context context, List<Value> arguments);
    }

    /// <summary>
    /// Built-in function value
    /// </summary>
    public class BuiltinFuncValue : FuncValue
    {
        public Function Func;

        public BuiltinFuncValue(Function func)
        {
            Func = func;
        }

        public override Value? Call(Context context, List<Value> arguments)
        {
            List<ValueType> receivedArgumentTypes = arguments.Select(a => a.Type).ToList();

            // Check that arguments match up to expected
            bool match = false;
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
            }

            if (!match)
            {
                string error = "cannot match argument types against a signature.";
                error += Environment.NewLine + "  Received: <" + string.Join(", ", receivedArgumentTypes.Select(a => a.ToString()).ToArray()) + ">";
                error += Environment.NewLine + "  Expected: " + string.Join(" | ", Func.ArgumentTypes.Select(a => "<" + string.Join(", ", a.Select(b => b.ToString())) + ">"));

                context.Error = new(0, 0, Error.Types.Type, error);
                return null;
            }

            Value value = Func.Call(context, arguments);

            return value;
        }

        public static BuiltinFuncValue From(Value value)
        {
            throw new NotImplementedException();
        }

        public override Value To(ValueType type)
        {
            return type switch
            {
                ValueType.INT => new IntValue(0),
                ValueType.FLOAT => new FloatValue(0),
                ValueType.STRING => new StringValue("function"),
                _ => throw new Exception("Unable to cast: unknown value type passed")
            };
        }
    }
}
