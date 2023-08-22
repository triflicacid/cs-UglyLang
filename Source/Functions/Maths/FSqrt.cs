using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.Maths
{
    public class FSqrt : Function, IDefinedGlobally
    {
        public FSqrt()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "SQRT";
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.FloatT };

            public OverloadOne()
            : base(Arguments, Type.FloatT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                double x = ((FloatValue)arguments[0]).Value;
                if (x < 0)
                {
                    context.Error = new(lineNo, colNo, Error.Types.Argument, "attempted square root of a negative number");
                    return Signal.ERROR;
                }

                FloatValue value = new(Math.Sqrt(x));
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
