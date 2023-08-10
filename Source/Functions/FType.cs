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
    /// Returns the type of the argument as a string
    /// </summary>
    public class FType : Function
    {

        private static readonly List<UnresolvedType[]> Arguments = new()
        {
            new UnresolvedType[] { ResolvedType.Any },
        };

        public FType() : base(Arguments, ResolvedType.Type) { }

        protected override Signal CallOverload(Context context, int _, List<Value> arguments, TypeParameterCollection c)
        {
            TypeValue value = new(arguments[0].Type);
            context.SetFunctionReturnValue(value);
            return Signal.NONE;
        }
    }
}
