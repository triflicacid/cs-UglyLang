using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.Maths
{
    /// <summary>
    /// Return the successor of the given integer
    /// </summary>
    public class FPred: Function
    {

        private static readonly List<Types.Type[]> ArgumentType = new()
        {
            new Types.Type[] { new IntType() },
        };

        public FPred() : base(ArgumentType, new IntType()) { }

        protected override Value CallOverload(Context context, int _, List<Value> arguments)
        {
            return new IntValue(((IntValue)arguments[0]).Value - 1);
        }
    }
}
