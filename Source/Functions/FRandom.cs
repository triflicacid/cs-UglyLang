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
            private readonly static Types.Type[] Arguments = Array.Empty<Types.Type>();

            public OverloadOne()
            : base(Arguments, Types.Type.FloatT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters)
            {
                double n = Generator.NextDouble();
                context.SetFunctionReturnValue(new FloatValue(n));
                return Signal.NONE;
            }
        }

        internal class OverloadTwo : FunctionOverload
        {
            private readonly static Types.Type[] Arguments = new Types.Type[] { Types.Type.FloatT };

            public OverloadTwo()
            : base(Arguments, Types.Type.FloatT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters)
            {
                double max = ((FloatValue)arguments[0]).Value;
                double n = Generator.NextDouble() * max;
                context.SetFunctionReturnValue(new FloatValue(n));
                return Signal.NONE;
            }
        }

        internal class OverloadThree : FunctionOverload
        {
            private readonly static Types.Type[] Arguments = new Types.Type[] { Types.Type.FloatT, Types.Type.FloatT };

            public OverloadThree()
            : base(Arguments, Types.Type.FloatT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters)
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
