using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

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
            private static readonly Type[] Arguments = new Type[] { Type.Map(Param) };

            public OverloadOne()
            : base(Arguments, Type.IntT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                IntValue value = new(((MapValue)arguments[0]).Value.Count);
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
