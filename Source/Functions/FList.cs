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
    public class FList : Function
    {
        private static readonly List<UnresolvedType[]> Arguments = new()
        {
            new UnresolvedType[] { ResolvedType.Type },
        };

        public FList() : base(Arguments, ResolvedType.List(new Any())) { }

        protected override Value? CallOverload(Context context, int _, List<Value> arguments, TypeParameterCollection c)
        {
            Types.Type type = ((TypeValue)arguments[0]).Value;
            return new ListValue(type);
        }
    }
}
