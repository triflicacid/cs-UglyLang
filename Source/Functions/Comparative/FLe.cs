using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions
{
    public class FLe : Function
    {

        private static readonly Values.ValueType[][] ArgumentType = new Values.ValueType[][]
        {
            new Values.ValueType[] { Values.ValueType.FLOAT, Values.ValueType.FLOAT },
        };

        public FLe() : base(ArgumentType, Values.ValueType.INT) { }

        public override Value Call(Context context, List<Value> arguments)
        {
            return new IntValue(((FloatValue)arguments[0]).Value <= ((FloatValue)arguments[1]).Value);
        }
    }
}
