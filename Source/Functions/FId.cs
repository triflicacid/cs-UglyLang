using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions
{
    public class FId : Function, IDefinedGlobally
    {
        public FId()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "ID";
        }



        internal class OverloadOne : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { new TypeParameter("a") };

            public OverloadOne()
            : base(Arguments, new TypeParameter("a"))
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                context.SetFunctionReturnValue(arguments[0]);
                return Signal.NONE;
            }
        }
    }
}
