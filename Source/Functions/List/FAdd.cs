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
    /// Function to add an item to the list
    /// </summary>
    public class FAdd : Function
    {
        private static readonly TypeParameter TypeParam = new("a"); // For convenience of repetition below
        private static readonly List<Types.Type[]> ArgumentType = new()
        {
            new Types.Type[] { new ListType(TypeParam), TypeParam },
            new Types.Type[] { new ListType(TypeParam), TypeParam, new IntType() },
        };

        public FAdd() : base(ArgumentType, new EmptyType()) { }

        protected override Value CallOverload(Context context, int index, List<Value> arguments)
        {
            ListValue list = (ListValue)arguments[0];

            if (index == 1)
            {
                // Add at the given index
                list.Value.Insert((int) ((IntValue)arguments[2]).Value, arguments[1]);
            }
            else
            {
                // Add to the end
                list.Value.Add(arguments[1]);
            }
            
            return new EmptyValue();
        }
    }
}
