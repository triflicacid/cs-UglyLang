using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.Comparative
{
    public class FEq : Function
    {

        private static readonly List<Types.Type[]> ArgumentType = new()
        {
            new Types.Type[] { new Any(), new Any() },
        };

        public FEq() : base(ArgumentType, new IntType()) { }

        protected override Value CallOverload(Context context, int _, List<Value> arguments)
        {
            Value a = arguments[0], b = arguments[1];
            bool eq;
            if (a == b) eq = true;
            else if (a.Type.Equals(b.Type))
            {
                if (a.Type is IntType) eq = ((IntValue)a).Value == ((IntValue)b).Value;
                else if (a.Type is FloatType) eq = ((FloatValue)a).Value == ((FloatValue)b).Value;
                else if (a.Type is StringType) eq = ((StringValue)a).Value == ((StringValue)b).Value;
                else eq = false;
            }
            else if (a.Type is IntType && b.Type is FloatType)
            {
                eq = ((IntValue)a).Value == ((FloatValue)b).Value;
            }
            else if (a.Type is FloatType && b.Type is IntType)
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
