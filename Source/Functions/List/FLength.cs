using UglyLang.Source.Types;
using UglyLang.Source.Values;

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
            private static readonly Types.Type[] Arguments = new Types.Type[] { Types.Type.List(Param) };

            public OverloadOne()
            : base(Arguments, Types.Type.IntT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters)
            {
                IntValue value = new(((ListValue)arguments[0]).Value.Count);
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
