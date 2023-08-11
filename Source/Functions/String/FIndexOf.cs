using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.String
{
    /// <summary>
    /// Function to get the index of a substring in a string
    /// </summary>
    public class FIndexOf : Function
    {
        private static readonly List<UnresolvedType[]> Arguments = new()
        {
            new UnresolvedType[] { ResolvedType.String, ResolvedType.String },
        };

        public FIndexOf() : base(Arguments, ResolvedType.Int) { }

        protected override Signal CallOverload(Context context, int _, List<Value> arguments, TypeParameterCollection c)
        {
            IntValue value = new(((StringValue)arguments[0]).Value.IndexOf(((StringValue)arguments[1]).Value));
            context.SetFunctionReturnValue(value);
            return Signal.NONE;
        }
    }
}
