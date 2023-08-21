using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.List
{
    /// <summary>
    /// Function to return the length of a list
    /// </summary>
    public class FLength : Function
    {
        private static readonly TypeParameter Param = new("a");

        public FLength()
        {
            Overloads.Add(new OverloadOne());
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.List(Param) };

            public OverloadOne()
            : base(Arguments, Type.IntT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                IntValue value = new(((ListValue)arguments[0]).Value.Count);
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
