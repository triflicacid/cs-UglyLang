using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.Comparative
{
    public class FEq : Function
    {
        private static readonly List<UnresolvedType[]> Arguments = new()
        {
            new UnresolvedType[] { ResolvedType.Any, ResolvedType.Any },
        };

        public FEq() : base(Arguments, ResolvedType.Int) { }

        protected override Signal CallOverload(Context context, int _, List<Value> arguments, TypeParameterCollection c)
        {
            IntValue value = new(arguments[0].Equals(arguments[1]));
            context.SetFunctionReturnValue(value);
            return Signal.NONE;
        }
    }
}
