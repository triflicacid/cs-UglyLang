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
    /// Function to reverse said list
    /// </summary>
    public class FReverse : Function
    {
        private static readonly UnresolvedType ListType = ResolvedType.List(new TypeParameter("a")); // For convenience of repetition below
        private static readonly List<UnresolvedType[]> Arguments = new()
        {
            new UnresolvedType[] { ListType }
        };

        public FReverse() : base(Arguments, ListType) { }

        protected override Signal CallOverload(Context context, int _, List<Value> arguments, TypeParameterCollection c)
        {
            ListValue list = (ListValue)arguments[0];
            List<Value> copy = new(list.Value);
            copy.Reverse();
            context.SetFunctionReturnValue(new ListValue(((ListType)list.Type).Member, copy));
            return Signal.NONE;
        }
    }
}
