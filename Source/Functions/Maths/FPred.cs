using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.Maths
{
    /// <summary>
    /// Return the successor of the given integer
    /// </summary>
    public class FPred: Function
    {

        private static readonly List<UnresolvedType[]> Arguments = new()
        {
            new UnresolvedType[] { ResolvedType.Int },
        };

        public FPred() : base(Arguments, ResolvedType.Int) { }

        protected override Signal CallOverload(Context context, int _, List<Value> arguments, TypeParameterCollection c)
        {
            IntValue value = new(((IntValue)arguments[0]).Value - 1);
            context.SetFunctionReturnValue(value);
            return Signal.NONE;
        }
    }
}
