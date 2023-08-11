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
    public class FSleep : Function, IDefinedGlobally
    {
        public FSleep()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "SLEEP";
        }



        internal class OverloadOne : FunctionOverload
        {
            private readonly static Types.Type[] Arguments = new Types.Type[] { Types.Type.IntT };

            public OverloadOne()
            : base(Arguments, Types.Type.EmptyT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters)
            {
                Thread.Sleep((int)((IntValue)arguments[0]).Value);
                return Signal.NONE;
            }
        }
    }
}
