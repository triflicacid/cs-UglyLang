using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.Maths
{
    /// <summary>
    /// Round the given float
    /// </summary>
    public class FRound : Function, IDefinedGlobally
    {
        public FRound()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "ROUND";
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
                context.SetFunctionReturnValue(new IntValue((long)Math.Round(value)));
                return Signal.NONE;
            }
        }
    }
}
