using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.Bitwise
{
    public class FNot : Function, IDefinedGlobally
    {
        public FNot()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "BITNOT";
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.IntT };

            public OverloadOne()
            : base(Arguments, Type.IntT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                IntValue a = (IntValue)arguments[0];
                IntValue value = new(~a.Value);
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
