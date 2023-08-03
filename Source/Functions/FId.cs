using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions
{
    public class FId : Function
    {
        private static readonly Values.ValueType[][] ArgumentType = new Values.ValueType[][]
        {
            new Values.ValueType[] { Values.ValueType.ANY },
        };

        public FId() : base(ArgumentType, Values.ValueType.ANY) { }

        public override Value Call(Context context, List<Value> arguments)
        {
            return arguments[0];
        }
    }
}
