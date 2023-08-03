using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.Comparative
{
    public class FGe : Function
    {

        private static readonly List<Values.ValueType[]> ArgumentType = new()
        {
            new Values.ValueType[] { Values.ValueType.FLOAT, Values.ValueType.FLOAT },
        };

        public FGe() : base(ArgumentType, Values.ValueType.INT) { }

        public override Value Call(Context context, int _, List<Value> arguments)
        {
            return new IntValue(((FloatValue)arguments[0]).Value >= ((FloatValue)arguments[1]).Value);
        }
    }
}
