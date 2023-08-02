using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions
{
    /// <summary>
    /// Return the successor of the given integer
    /// </summary>
    public class FSucc : Function
    {

        private static readonly Values.ValueType[][] ArgumentType = new Values.ValueType[][]
        {
            new Values.ValueType[] { Values.ValueType.INT },
        };

        public FSucc() : base(ArgumentType, Values.ValueType.INT) { }

        public override Value Call(Context context, List<Value> arguments)
        {
            return new IntValue(((IntValue)arguments[0]).Value + 1);
        }
    }
}
