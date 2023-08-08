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
    /// Function to remove any itm from a list
    /// </summary>
    public class FRemove : Function
    {
        private static readonly List<Types.Type[]> ArgumentType = new()
        {
            new Types.Type[] { new ListType(new TypeParameter("a")), new TypeParameter("a") },
        };

        public FRemove() : base(ArgumentType, new IntType()) { }

        protected override Value CallOverload(Context context, int _, List<Value> arguments)
        {
            ListValue list = (ListValue)arguments[0];
            return new IntValue(list.Remove(arguments[1]));
        }
    }
}
