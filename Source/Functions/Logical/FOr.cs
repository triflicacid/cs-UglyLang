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
    public class FOr : Function
    {

        private static readonly List<UnresolvedType[]> Arguments = new()
        {
            new UnresolvedType[] { ResolvedType.Any, ResolvedType.Any },
        };

        public FOr() : base(Arguments, ResolvedType.Int) { }

        protected override Signal CallOverload(Context context, int _, List<Value> arguments, TypeParameterCollection c)
        {
            Value a = arguments[0], b = arguments[1];
            IntValue value = new(a.IsTruthy() || b.IsTruthy());
            context.SetFunctionReturnValue(value);
            return Signal.NONE;
        }
    }
}
