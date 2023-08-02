using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions
{
    /// <summary>
    /// Concatenates two items as strings
    /// </summary>
    public class FConcat : Function
    {

        private static readonly Values.ValueType[][] ArgumentType = new Values.ValueType[][]
        {
            new Values.ValueType[] { Values.ValueType.ANY, Values.ValueType.ANY },
        };

        public FConcat() : base(ArgumentType, Values.ValueType.STRING) { }

        public override Value Call(Context context, List<Value> arguments)
        {
            return new StringValue(StringValue.From(arguments[0]).Value + StringValue.From(arguments[1]).Value);
        }
    }
}
