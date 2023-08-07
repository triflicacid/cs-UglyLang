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
    public class FConcat : Function
    {

        private static readonly List<Types.Type[]> ArgumentType = new()
        {
            new Types.Type[] { new Any(), new Any() },
        };

        public FConcat() : base(ArgumentType, new StringType()) { }

        protected override Value CallOverload(Context context, int _, List<Value> arguments)
        {
            return new StringValue(StringValue.From(arguments[0]).Value + StringValue.From(arguments[1]).Value);
        }
    }
}
