﻿using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.Maths
{
    /// <summary>
    /// Return the successor of the given integer
    /// </summary>
    public class FPred : Function, IDefinedGlobally
    {
        public FPred()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "PRED";
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Types.Type[] Arguments = new Types.Type[] { Types.Type.IntT };

            public OverloadOne()
            : base(Arguments, Types.Type.IntT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters)
            {
                IntValue value = new(((IntValue)arguments[0]).Value - 1);
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
