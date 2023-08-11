using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.Maths
{
    public class FExp : Function, IDefinedGlobally
    {
        public FExp()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "EXP";
        }


        internal class OverloadOne : FunctionOverload
        {
            private readonly static Types.Type[] Arguments = new Types.Type[] { Types.Type.FloatT, Types.Type.FloatT };

            public OverloadOne()
            : base(Arguments, Types.Type.FloatT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters)
            {
                FloatValue value = new(Math.Pow(((FloatValue)arguments[0]).Value, ((FloatValue)arguments[1]).Value));
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
