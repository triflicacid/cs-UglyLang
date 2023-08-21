using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.Logical
{
    public class FAnd : Function, IDefinedGlobally
    {
        public FAnd()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "AND";
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.AnyT, Type.AnyT };

            public OverloadOne()
            : base(Arguments, Type.IntT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                Value a = arguments[0], b = arguments[1];
                IntValue value = new(a.IsTruthy() && b.IsTruthy());
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
