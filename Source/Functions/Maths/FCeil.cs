﻿using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.Maths
{
    /// <summary>
    /// Return the ceiling of the given float
    /// </summary>
    public class FCeil : Function, IDefinedGlobally
    {
        public FCeil()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "CEIL";
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.FloatT };

            public OverloadOne()
            : base(Arguments, Type.IntT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                double value = ((FloatValue)arguments[0]).Value;
                context.SetFunctionReturnValue(new IntValue((long)Math.Ceiling(value)));
                return Signal.NONE;
            }
        }
    }
}
