using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.Comparative
{
    public class FEq : Function
    {

        private static readonly List<Values.ValueType[]> ArgumentType = new()
        {
            new Values.ValueType[] { Values.ValueType.ANY, Values.ValueType.ANY },
        };

        public FEq() : base(ArgumentType, Values.ValueType.INT) { }

        public override Value Call(Context context, int _, List<Value> arguments)
        {
            Value a = arguments[0], b = arguments[1];
            bool eq;
            if (a == b) eq = true;
            else if (a.Type == b.Type)
            {
                eq = a.Type switch
                {
                    Values.ValueType.INT => ((IntValue)a).Value == ((IntValue)b).Value,
                    Values.ValueType.FLOAT => ((FloatValue)a).Value == ((FloatValue)b).Value,
                    Values.ValueType.STRING => ((StringValue)a).Value == ((StringValue)b).Value,
                    _ => false,
                };
            }
            else if (a.Type == Values.ValueType.INT && b.Type == Values.ValueType.FLOAT)
            {
                eq = ((IntValue)a).Value == ((FloatValue)b).Value;
            }
            else if (a.Type == Values.ValueType.FLOAT && b.Type == Values.ValueType.INT)
            {
                eq = ((FloatValue)a).Value == ((IntValue)b).Value;
            }
            else
            {
                eq = false;
            }

            return new IntValue(eq);
        }
    }
}
