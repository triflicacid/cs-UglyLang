using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.Logical
{
    public class FXOr : Function
    {

        private static readonly List<UnresolvedType[]> Arguments = new()
        {
            new UnresolvedType[] { ResolvedType.Any, ResolvedType.Any },
        };

        public FXOr() : base(Arguments, ResolvedType.Int) { }

        protected override Signal CallOverload(Context context, int _, List<Value> arguments, TypeParameterCollection c)
        {
            bool a = arguments[0].IsTruthy(), b = arguments[1].IsTruthy();
            IntValue value = new((a && !b) || (!a && b));
            context.SetFunctionReturnValue(value);
            return Signal.NONE;
        }
    }
}
