using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.Map
{
    /// <summary>
    /// Function to set the key to the given value
    /// </summary>
    public class FSet : Function
    {
        private static readonly TypeParameter Param = new("a");

        public FSet()
        {
            Overloads.Add(new OverloadOne());
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.Map(Param), Type.StringT, Param };

            public OverloadOne()
            : base(Arguments, Type.EmptyT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                MapValue map = (MapValue)arguments[0];
                string key = ((StringValue)arguments[1]).Value;

                if (map.Value.ContainsKey(key))
                {
                    map.Value[key] = arguments[2];
                }
                else
                {
                    map.Value.Add(key, arguments[2]);
                }

                return Signal.NONE;
            }
        }
    }
}
