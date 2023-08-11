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
    public class FNot : Function, IDefinedGlobally
    {
        public FNot()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "NOT";
        }


        internal class OverloadOne : FunctionOverload
        {
            private readonly static Types.Type[] Arguments = new Types.Type[] { Types.Type.AnyT };

            public OverloadOne()
            : base(Arguments, Types.Type.IntT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters)
            {
                context.SetFunctionReturnValue(new IntValue(!arguments[0].IsTruthy()));
                return Signal.NONE;
            }
        }
    }
}
