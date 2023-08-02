using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.Maths
{
    public class FExp : Function
    {

        private static readonly Values.ValueType[][] ArgumentType = new Values.ValueType[][]
        {
            new Values.ValueType[] { Values.ValueType.FLOAT, Values.ValueType.FLOAT },
        };

        public FExp() : base(ArgumentType, Values.ValueType.FLOAT) { }

        public override Value Call(Context context, List<Value> arguments)
        {
            return new FloatValue(Math.Pow(((FloatValue)arguments[0]).Value, ((FloatValue)arguments[1]).Value));
        }
    }
}
