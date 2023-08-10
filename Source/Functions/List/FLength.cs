using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.List
{
    /// <summary>
    /// Function to return the length of a list
    /// </summary>
    public class FLength : Function
    {
        private static readonly List<UnresolvedType[]> Arguments = new()
        {
            new UnresolvedType[] { ResolvedType.List(new TypeParameter("a")) },
        };

        public FLength() : base(Arguments, ResolvedType.Int) { }

        protected override Signal CallOverload(Context context, int _, List<Value> arguments, TypeParameterCollection c)
        {
            IntValue value = new(((ListValue)arguments[0]).Value.Count);
            context.SetFunctionReturnValue(value);
            return Signal.NONE;
        }
    }
}
