using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.Map
{
    /// <summary>
    /// Function to return whether the map contains the given key
    /// </summary>
    public class FHas : Function
    {
        private static readonly TypeParameter Param = new("a");

        public FHas()
        {
            Overloads.Add(new OverloadOne());
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Types.Type[] Arguments = new Types.Type[] { Types.Type.Map(Param), Types.Type.StringT };

            public OverloadOne()
            : base(Arguments, Types.Type.IntT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters)
            {
                MapValue map = (MapValue)arguments[0];
                string key = ((StringValue)arguments[1]).Value;
                context.SetFunctionReturnValue(new IntValue(map.Value.ContainsKey(key)));
                return Signal.NONE;
            }
        }
    }
}
