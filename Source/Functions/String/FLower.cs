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
    /// Function to return the lower-case version of the string
    /// </summary>
    public class FLower : Function
    {
        private static readonly List<UnresolvedType[]> Arguments = new()
        {
            new UnresolvedType[] { ResolvedType.String },
        };

        public FLower() : base(Arguments, ResolvedType.String) { }

        protected override Signal CallOverload(Context context, int _, List<Value> arguments, TypeParameterCollection c)
        {
            StringValue value = new(((StringValue)arguments[0]).Value.ToLower());
            context.SetFunctionReturnValue(value);
            return Signal.NONE;
        }
    }
}
