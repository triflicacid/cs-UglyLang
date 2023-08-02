using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions
{
    /// <summary>
    /// Returns the type of the argument as a string
    /// </summary>
    public class FType : Function
    {

        private static readonly Values.ValueType[][] ArgumentType = new Values.ValueType[][]
        {
            new Values.ValueType[] { Values.ValueType.ANY },
        };

        public FType() : base(ArgumentType, Values.ValueType.STRING) { }

        public override Value Call(Context context, List<Value> arguments)
        {
            return new StringValue(arguments[0].Type.ToString());
        }
    }
}
