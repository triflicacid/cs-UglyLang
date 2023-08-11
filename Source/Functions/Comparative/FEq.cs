using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.Comparative
{
    public class FEq : Function, IDefinedGlobally
    {
        public FEq()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "EQ";
        }



        internal class OverloadOne : FunctionOverload
        {
            private readonly static Types.Type[] Arguments = new Types.Type[] { Types.Type.AnyT, Types.Type.AnyT };

            public OverloadOne()
            : base(Arguments, Types.Type.IntT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters)
            {
                IntValue value = new(arguments[0].Equals(arguments[1]));
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
