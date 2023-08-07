using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.Maths
{
    public class FExp : Function
    {

        private static readonly List<Types.Type[]> ArgumentType = new()
        {
            new Types.Type[] { new FloatType(), new FloatType() },
        };

        public FExp() : base(ArgumentType, new FloatType()) { }

        protected override Value CallOverload(Context context, int _, List<Value> arguments)
        {
            return new FloatValue(Math.Pow(((FloatValue)arguments[0]).Value, ((FloatValue)arguments[1]).Value));
        }
    }
}
