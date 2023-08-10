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
    public class FNeg : Function
    {

        private static readonly List<UnresolvedType[]> Arguments = new()
        {
            new UnresolvedType[] { ResolvedType.Float },
        };

        public FNeg() : base(Arguments, ResolvedType.Float) { }

        protected override Signal CallOverload(Context context, int _, List<Value> arguments, TypeParameterCollection c)
        {
            FloatValue value = new(-((FloatValue)arguments[0]).Value);
            context.SetFunctionReturnValue(value);
            return Signal.NONE;
        }
    }
}
