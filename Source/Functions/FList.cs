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
    /// Function to create a list of said type
    /// </summary>
    public class FList : Function, IDefinedGlobally
    {
        private static readonly List<UnresolvedType[]> Arguments = new()
        {
            new UnresolvedType[] { ResolvedType.Type },
        };

        public FList() : base(Arguments, ResolvedType.List(new Any())) { }

        public string GetDefinedName()
        {
            return "LIST";
        }

        protected override Signal CallOverload(Context context, int _, List<Value> arguments, TypeParameterCollection c)
        {
            Types.Type type = ((TypeValue)arguments[0]).Value;
            context.SetFunctionReturnValue(new ListValue(type));
            return Signal.NONE;
        }
    }
}
