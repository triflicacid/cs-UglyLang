﻿using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.Logical
{
    public class FOr : Function, IDefinedGlobally
    {
        public FOr()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "OR";
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Types.Type[] Arguments = new Types.Type[] { Types.Type.AnyT, Types.Type.AnyT };

            public OverloadOne()
            : base(Arguments, Types.Type.IntT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters)
            {
                Value a = arguments[0], b = arguments[1];
                IntValue value = new(a.IsTruthy() || b.IsTruthy());
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
