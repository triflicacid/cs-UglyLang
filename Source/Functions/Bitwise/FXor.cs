using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.Bitwise
{
    public class FXor : Function, IDefinedGlobally
    {
        public FXor()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "BITXOR";
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.IntT, Type.IntT };

            public OverloadOne()
            : base(Arguments, Type.IntT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                IntValue a = (IntValue)arguments[0], b = (IntValue)arguments[1];
                IntValue value = new(a.Value ^ b.Value);
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
