using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions
{
    /// <summary>
    /// Concatenates two items as strings
    /// </summary>
    public class FConcat : Function, IDefinedGlobally
    {
        private static readonly List<UnresolvedType[]> Arguments = new()
        {
            new UnresolvedType[] { ResolvedType.Any, ResolvedType.Any },
        };

        public FConcat() : base(Arguments, ResolvedType.String) { }

        public string GetDefinedName()
        {
            return "CONCAT";
        }

        protected override Signal CallOverload(Context context, int _, List<Value> arguments, TypeParameterCollection c)
        {
            StringValue value = new(StringValue.From(arguments[0]).Value + StringValue.From(arguments[1]).Value);
            context.SetFunctionReturnValue(value);
            return Signal.NONE;
        }
    }
}
