using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.Types
{
    /// <summary>
    /// Returns the empty version of a type
    /// </summary>
    public class FEmpty : Function, IDefinedGlobally
    {
        public FEmpty()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "EMPTY";
        }

        internal class OverloadOne : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.TypeT };

            public OverloadOne()
            : base(Arguments, Type.AnyT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                context.SetFunctionReturnValue(new EmptyValue(((TypeValue)arguments[0]).Value));
                return Signal.NONE;
            }
        }
    }
}
