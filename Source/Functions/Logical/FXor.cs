using UglyLang.Source.Types;
using UglyLang.Source.Values;

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
            private static readonly Types.Type[] Arguments = new Types.Type[] { Types.Type.AnyT, Types.Type.AnyT };

            public OverloadOne()
            : base(Arguments, Types.Type.IntT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters)
            {
                bool a = arguments[0].IsTruthy(), b = arguments[1].IsTruthy();
                IntValue value = new((a && !b) || (!a && b));
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
