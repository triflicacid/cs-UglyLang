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
    /// Returns the type of the argument as a string
    /// </summary>
    public class FType : Function
    {

        private static readonly List<Types.Type[]> ArgumentType = new()
        {
            new Types.Type[] { new Any() },
        };

        public FType() : base(ArgumentType, new TypeType()) { }

        protected override Value CallOverload(Context context, int _, List<Value> arguments)
        {
            return new TypeValue(arguments[0].Type);
        }
    }
}
