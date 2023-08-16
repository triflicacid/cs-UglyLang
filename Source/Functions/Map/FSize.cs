using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.Map
{
    /// <summary>
    /// Function to return the size of a map
    /// </summary>
    public class FSize : Function
    {
        private static readonly TypeParameter Param = new("a");

        public FSize()
        {
            Overloads.Add(new OverloadOne());
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Types.Type[] Arguments = new Types.Type[] { Types.Type.Map(Param) };

            public OverloadOne()
            : base(Arguments, Types.Type.IntT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters)
            {
                IntValue value = new(((MapValue)arguments[0]).Value.Count);
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
