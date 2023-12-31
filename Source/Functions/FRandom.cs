﻿using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions
{
    /// <summary>
    /// Returns a random floating number in a range, depending on the parameters passed.
    /// </summary>
    public class FRandom : Function, IDefinedGlobally
    {
        private static readonly Random Generator = new();

        public FRandom()
        {
            Overloads.Add(new OverloadOne());
            Overloads.Add(new OverloadTwo());
            Overloads.Add(new OverloadThree());
        }

        public string GetDefinedName()
        {
            return "RANDOM";
        }



        internal class OverloadOne : FunctionOverload
        {
            private static readonly Type[] Arguments = Array.Empty<Type>();

            public OverloadOne()
            : base(Arguments, Type.FloatT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                double n = Generator.NextDouble();
                context.SetFunctionReturnValue(new FloatValue(n));
                return Signal.NONE;
            }
        }

        internal class OverloadTwo : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.FloatT };

            public OverloadTwo()
            : base(Arguments, Type.FloatT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                double max = ((FloatValue)arguments[0]).Value;
                double n = Generator.NextDouble() * max;
                context.SetFunctionReturnValue(new FloatValue(n));
                return Signal.NONE;
            }
        }

        internal class OverloadThree : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.FloatT, Type.FloatT };

            public OverloadThree()
            : base(Arguments, Type.FloatT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                double min = ((FloatValue)arguments[0]).Value;
                double max = ((FloatValue)arguments[1]).Value;
                double n = min + Generator.NextDouble() * (max - min);
                context.SetFunctionReturnValue(new FloatValue(n));
                return Signal.NONE;
            }
        }
    }
}
