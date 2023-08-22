using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.Logical
{
    public class FIf : Function, IDefinedGlobally
    {
        public FIf()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "IF";
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly TypeParameter Param = new("a");
            private static readonly Type[] Arguments = new Type[] { Type.AnyT, Param, Param };

            public OverloadOne()
            : base(Arguments, Param)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                Value value = arguments[0].IsTruthy() ? arguments[1] : arguments[2];
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
