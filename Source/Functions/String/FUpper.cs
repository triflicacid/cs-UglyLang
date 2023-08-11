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
    /// Function to return the upper-case version of the string
    /// </summary>
    public class FUpper : Function
    {
        private static readonly List<UnresolvedType[]> Arguments = new()
        {
            new UnresolvedType[] { ResolvedType.String },
        };

        public FUpper() : base(Arguments, ResolvedType.String) { }

        protected override Signal CallOverload(Context context, int _, List<Value> arguments, TypeParameterCollection c)
        {
            StringValue value = new(((StringValue)arguments[0]).Value.ToUpper());
            context.SetFunctionReturnValue(value);
            return Signal.NONE;
        }
    }
}
