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

        private static readonly List<UnresolvedType[]> Arguments = new()
        {
            new UnresolvedType[] { ResolvedType.Int },
        };

        public FSleep() : base(Arguments, ResolvedType.Empty) { }

        protected override Signal CallOverload(Context context, int _, List<Value> arguments, TypeParameterCollection c)
        {
            Thread.Sleep((int)((IntValue)arguments[0]).Value);
            return Signal.NONE;
        }
    }
}
