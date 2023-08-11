using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.Comparative
{
    public class FLe : Function, IDefinedGlobally
    {
        public FLe()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "LE";
        }



        internal class OverloadOne : FunctionOverload
        {
            private readonly static Types.Type[] Arguments = new Types.Type[] { Types.Type.FloatT, Types.Type.FloatT };

            public OverloadOne()
            : base(Arguments, Types.Type.IntT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters)
            {
                IntValue value = new(((FloatValue)arguments[0]).Value <= ((FloatValue)arguments[1]).Value);
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
