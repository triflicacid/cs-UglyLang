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
    public class FNot : Function, IDefinedGlobally
    {

        private static readonly List<UnresolvedType[]> Arguments = new()
        {
            new UnresolvedType[] { ResolvedType.Any },
        };

        public FNot() : base(Arguments, ResolvedType.Int) { }

        public string GetDefinedName()
        {
            return "NOT";
        }

        protected override Signal CallOverload(Context context, int _, List<Value> arguments, TypeParameterCollection c)
        {
            context.SetFunctionReturnValue(new IntValue(!arguments[0].IsTruthy()));
            return Signal.NONE;
        }
    }
}
