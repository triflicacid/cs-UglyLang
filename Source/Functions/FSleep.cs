using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions
{
    /// <summary>
    /// Sleep the current thread for some milliseconds
    /// </summary>
    public class FSleep : Function
    {

        private static readonly List<Values.ValueType[]> ArgumentType = new()
        {
            new Values.ValueType[] { Values.ValueType.INT },
        };

        public FSleep() : base(ArgumentType, Values.ValueType.EMPTY) { }

        public override Value Call(Context context, int _, List<Value> arguments)
        {
            Thread.Sleep((int)((IntValue)arguments[0]).Value);
            return new EmptyValue();
        }
    }
}
