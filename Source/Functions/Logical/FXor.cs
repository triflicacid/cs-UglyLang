using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.Logical
{
    public class FXOr : Function
    {

        private static readonly List<Values.ValueType[]> ArgumentType = new()
        {
            new Values.ValueType[] { Values.ValueType.ANY, Values.ValueType.ANY },
        };

        public FXOr() : base(ArgumentType, Values.ValueType.INT) { }

        public override Value Call(Context context, int _, List<Value> arguments)
        {
            bool a = arguments[0].IsTruthy(), b = arguments[1].IsTruthy();
            return new IntValue((a && !b) || (!a && b));
        }
    }
}
