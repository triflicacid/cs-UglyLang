using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.Types
{
    /// <summary>
    /// Function to return whether the given value is an instance of said type
    /// </summary>
    public class FIs : Function, IDefinedGlobally
    {
        public FIs()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "IS";
        }



        internal class OverloadOne : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.AnyT, Type.TypeT };

            public OverloadOne()
            : base(Arguments, Type.IntT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                Type t = ((TypeValue)arguments[1]).Value;
                bool b = t is TypeType || t.IsTypeOf(arguments[0]); // Check if type of. Note, every type is an instance of TYPE.
                context.SetFunctionReturnValue(new IntValue(b));
                return Signal.NONE;
            }
        }
    }
}
