using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.Logical
{
    public class FAnd : Function
    {

        private static readonly List<Values.ValueType[]> ArgumentType = new()
        {
            new Values.ValueType[] { Values.ValueType.ANY, Values.ValueType.ANY },
        };

        public FAnd() : base(ArgumentType, Values.ValueType.INT) { }

        public override Value Call(Context context, int _, List<Value> arguments)
        {
            Value a = arguments[0], b = arguments[1];
            return new IntValue(a.IsTruthy() && b.IsTruthy());
        }
    }
}
