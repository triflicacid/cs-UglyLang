using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.Maths
{
    public class FMod : Function, IDefinedGlobally
    {
        public FMod()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "MOD";
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.FloatT, Type.FloatT };

            public OverloadOne()
            : base(Arguments, Type.FloatT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                FloatValue value = new(((FloatValue)arguments[0]).Value % ((FloatValue)arguments[1]).Value);
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
