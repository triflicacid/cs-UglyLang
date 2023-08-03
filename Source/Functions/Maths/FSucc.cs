using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.Maths
{
    /// <summary>
    /// Return the successor of the given integer
    /// </summary>
    public class FSucc : Function
    {

        private static readonly List<Values.ValueType[]> ArgumentType = new()
        {
            new Values.ValueType[] { Values.ValueType.INT },
        };

        public FSucc() : base(ArgumentType, Values.ValueType.INT) { }

        public override Value Call(Context context, int _, List<Value> arguments)
        {
            return new IntValue(((IntValue)arguments[0]).Value + 1);
        }
    }
}
