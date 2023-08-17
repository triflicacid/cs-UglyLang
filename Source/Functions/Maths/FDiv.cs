using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.Maths
{
    public class FDiv : Function, IDefinedGlobally
    {
        public FDiv()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "DIV";
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Types.Type[] Arguments = new Types.Type[] { Types.Type.FloatT, Types.Type.FloatT };

            public OverloadOne()
            : base(Arguments, Types.Type.FloatT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                double a = ((FloatValue)arguments[0]).Value, b = ((FloatValue)arguments[1]).Value;
                if (b == 0)
                {
                    context.Error = new(lineNo, colNo, Error.Types.Argument, "attempted division by zero");
                    return Signal.ERROR;
                }

                FloatValue value = new(a / b);
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
