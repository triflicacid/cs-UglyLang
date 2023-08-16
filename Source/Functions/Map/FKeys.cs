using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.Map
{
    /// <summary>
    /// Function to return a list of keys
    /// </summary>
    public class FKeys : Function
    {
        private static readonly TypeParameter Param = new("a");

        public FKeys()
        {
            Overloads.Add(new OverloadOne());
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Types.Type[] Arguments = new Types.Type[] { Types.Type.Map(Param) };

            public OverloadOne()
            : base(Arguments, Types.Type.List(Types.Type.StringT))
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters)
            {
                MapValue map = (MapValue)arguments[0];
                List<Value> keys = map.Value.Keys.Select(s => new StringValue(s)).ToList<Value>();
                context.SetFunctionReturnValue(new ListValue(Types.Type.StringT, keys));
                return Signal.NONE;
            }
        }
    }
}
