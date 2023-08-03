using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.Maths
{
    public class FNeg : Function
    {

        private static readonly List<Values.ValueType[]> ArgumentType = new()
        {
            new Values.ValueType[] { Values.ValueType.FLOAT },
        };

        public FNeg() : base(ArgumentType, Values.ValueType.FLOAT) { }

        public override Value Call(Context context, int _, List<Value> arguments)
        {
            return new FloatValue(-((FloatValue)arguments[0]).Value);
        }
    }
}
