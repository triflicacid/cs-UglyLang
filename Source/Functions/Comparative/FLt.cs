using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.Comparative
{
    public class FLt : Function
    {

        private static readonly List<UnresolvedType[]> Arguments = new()
        {
            new UnresolvedType[] { ResolvedType.Float, ResolvedType.Float },
        };

        public FLt() : base(Arguments, ResolvedType.Int) { }

        protected override Signal CallOverload(Context context, int _, List<Value> arguments, TypeParameterCollection c)
        {
            IntValue value = new(((FloatValue)arguments[0]).Value < ((FloatValue)arguments[1]).Value);
            context.SetFunctionReturnValue(value);
            return Signal.NONE;
        }
    }
}
