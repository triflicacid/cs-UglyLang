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
        private static readonly List<Values.ValueType[]> ArgumentType = new()
        {
            new Values.ValueType[] { Values.ValueType.ANY },
        };

        public FId() : base(ArgumentType, Values.ValueType.ANY) { }

        public override Value Call(Context context, int _, List<Value> arguments)
        {
            return arguments[0];
        }
    }
}
