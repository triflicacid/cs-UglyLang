using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.Maths
{
    /// <summary>
    /// Return the absolute of the given number
    /// </summary>
    public class FAbs : Function, IDefinedGlobally
    {
        public FAbs()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "ABS";
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.FloatT };

            public OverloadOne()
            : base(Arguments, Type.FloatT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                double value = ((FloatValue)arguments[0]).Value;
                context.SetFunctionReturnValue(new FloatValue(Math.Abs(value)));
                return Signal.NONE;
            }
        }
    }
}
