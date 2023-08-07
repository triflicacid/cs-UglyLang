using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions
{
    /// <summary>
    /// Sleep the current thread for some milliseconds
    /// </summary>
    public class FSleep : Function
    {

        private static readonly List<Types.Type[]> ArgumentType = new()
        {
            new Types.Type[] { new IntType() },
        };

        public FSleep() : base(ArgumentType, new None()) { }

        protected override Value CallOverload(Context context, int _, List<Value> arguments)
        {
            Thread.Sleep((int)((IntValue)arguments[0]).Value);
            return new EmptyValue();
        }
    }
}
