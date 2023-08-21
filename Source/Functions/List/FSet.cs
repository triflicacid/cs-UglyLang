using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.List
{
    /// <summary>
    /// Function to set an item from the list
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
            private static readonly Type[] Arguments = new Type[] { Type.List(Param), Type.IntT, Param };

            public OverloadOne()
            : base(Arguments, Type.EmptyT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                ListValue list = (ListValue)arguments[0];
                int index = (int)((IntValue)arguments[1]).Value;

                if (index >= 0 && index < list.Value.Count)
                {
                    list.Value[index] = arguments[2];
                    return Signal.NONE;
                }
                else
                {
                    context.Error = new(lineNo, colNo, Error.Types.Argument, string.Format("index {0} is out of bounds for {1}", index, list.Type));
                    return Signal.ERROR;
                }
            }
        }
    }
}
