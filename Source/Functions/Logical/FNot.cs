using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.Logical
{
    public class FNot : Function
    {

        private static readonly Values.ValueType[][] ArgumentType = new Values.ValueType[][]
        {
            new Values.ValueType[] { Values.ValueType.ANY },
        };

        public FNot() : base(ArgumentType, Values.ValueType.INT) { }

        public override Value Call(Context context, List<Value> arguments)
        {
            return new IntValue(!arguments[0].IsTruthy());
        }
    }
}
