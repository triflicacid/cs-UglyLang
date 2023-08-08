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
    /// Function to find the index of an item in the list
    /// </summary>
    public class FIndexOf : Function
    {
        private static readonly List<Types.Type[]> ArgumentType = new()
        {
            new Types.Type[] { new ListType(new TypeParameter("a")), new TypeParameter("a") },
        };

        public FIndexOf() : base(ArgumentType, new IntType()) { }

        protected override Value CallOverload(Context context, int _, List<Value> arguments)
        {
            return new IntValue(((ListValue)arguments[0]).IndexOf(arguments[1]));
        }
    }
}
