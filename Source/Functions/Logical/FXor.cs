using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.Logical
{
    public class FXOr : Function, IDefinedGlobally
    {
        public FXOr()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "XOR";
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.AnyT, Type.AnyT };

            public OverloadOne()
            : base(Arguments, Type.IntT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                bool a = arguments[0].IsTruthy(), b = arguments[1].IsTruthy();
                IntValue value = new((a && !b) || (!a && b));
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
