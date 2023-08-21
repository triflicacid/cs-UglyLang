using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.Logical
{
    public class FOr : Function, IDefinedGlobally
    {
        public FOr()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "OR";
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
                IntValue value = new(a.IsTruthy() || b.IsTruthy());
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
