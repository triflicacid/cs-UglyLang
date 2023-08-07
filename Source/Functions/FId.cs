using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions
{
    public class FId : Function
    {
        private static readonly List<Types.Type[]> ArgumentType = new()
        {
            new Types.Type[] { new Any() },
        };

        public FId() : base(ArgumentType, new Any()) { }

        protected override Value CallOverload(Context context, int _, List<Value> arguments)
        {
            return arguments[0];
        }
    }
}
