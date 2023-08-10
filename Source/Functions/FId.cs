using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions
{
    public class FId : Function
    {
        private static readonly List<UnresolvedType[]> Arguments = new()
        {
            new UnresolvedType[] { ResolvedType.Param("a") },
        };

        public FId() : base(Arguments, ResolvedType.Param("a")) { }

        protected override Signal CallOverload(Context context, int _, List<Value> arguments, TypeParameterCollection c)
        {
            context.SetFunctionReturnValue(arguments[0]);
            return Signal.NONE;
        }
    }
}
