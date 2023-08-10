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

        protected override Value CallOverload(Context context, int _, List<Value> arguments, TypeParameterCollection c)
        {
            return new IntValue(((IntValue)arguments[0]).Value - 1);
        }
    }
}
