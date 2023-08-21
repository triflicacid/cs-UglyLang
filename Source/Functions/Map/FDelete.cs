using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.Map
{
    /// <summary>
    /// Function to delete a key from the map
    /// </summary>
    public class FDelete : Function
    {
        private static readonly TypeParameter Param = new("a");

        public FDelete()
        {
            Overloads.Add(new OverloadOne());
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.Map(Param), Type.StringT };

            public OverloadOne()
            : base(Arguments, Param)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                MapValue map = (MapValue)arguments[0];
                string key = ((StringValue)arguments[1]).Value;

                if (map.Value.ContainsKey(key))
                {
                    context.SetFunctionReturnValue(map.Value[key]);
                    map.Value.Remove(key);
                    return Signal.NONE;
                }
                else
                {
                    context.Error = new(lineNo, colNo, Error.Types.Argument, string.Format("map does not contain key '{0}'", key));
                    return Signal.ERROR;
                }
            }
        }
    }
}
