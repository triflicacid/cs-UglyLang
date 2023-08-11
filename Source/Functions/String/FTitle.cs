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
    /// Function to return the title-case version of the string
    /// </summary>
    public class FTitle : Function
    {
        private static readonly List<UnresolvedType[]> Arguments = new()
        {
            new UnresolvedType[] { ResolvedType.String },
        };

        public FTitle() : base(Arguments, ResolvedType.String) { }

        protected override Signal CallOverload(Context context, int _, List<Value> arguments, TypeParameterCollection c)
        {
            StringValue value = new(string.Join(" ", ((StringValue)arguments[0]).Value.Split(" ").Select(s => s[0].ToString().ToUpper() + s[1..].ToLower())));
            context.SetFunctionReturnValue(value);
            return Signal.NONE;
        }
    }
}
