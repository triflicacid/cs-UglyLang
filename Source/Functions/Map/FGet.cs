using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.Map
{
    /// <summary>
    /// Function to return the item related to the given key
    /// </summary>
    public class FGet : Function
    {
        private static readonly TypeParameter Param = new("a");

        public FGet()
        {
            Overloads.Add(new OverloadOne());
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Types.Type[] Arguments = new Types.Type[] { Types.Type.Map(Param), Types.Type.StringT };

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
