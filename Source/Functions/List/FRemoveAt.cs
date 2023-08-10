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
    /// Function to remove an item at the given index from the list
    /// </summary>
    public class FRemoveAt : Function
    {
        private static readonly List<UnresolvedType[]> Arguments = new()
        {
            new UnresolvedType[] { ResolvedType.List(new TypeParameter("a")), ResolvedType.Int },
        };

        public FRemoveAt() : base(Arguments, ResolvedType.Int) { }

        protected override Value CallOverload(Context context, int _, List<Value> arguments, TypeParameterCollection c)
        {
            ListValue list = (ListValue)arguments[0];
            return new IntValue(list.RemoveAt((int) ((IntValue)arguments[1]).Value));
        }
    }
}
