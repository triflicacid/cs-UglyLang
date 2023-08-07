using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.Logical
{
    public class FOr : Function
    {

        private static readonly List<Types.Type[]> ArgumentType = new()
        {
            new Types.Type[] { new Any(), new Any() },
        };

        public FOr() : base(ArgumentType, new IntType()) { }

        protected override Value CallOverload(Context context, int _, List<Value> arguments)
        {
            Value a = arguments[0], b = arguments[1];
            return new IntValue(a.IsTruthy() || b.IsTruthy());
        }
    }
}
