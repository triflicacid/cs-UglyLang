using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.Logical
{
    public class FNot : Function, IDefinedGlobally
    {
        public FNot()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "NOT";
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.AnyT };

            public OverloadOne()
            : base(Arguments, Type.IntT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                context.SetFunctionReturnValue(new IntValue(!arguments[0].IsTruthy()));
                return Signal.NONE;
            }
        }
    }
}
