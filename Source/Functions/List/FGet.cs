using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.List
{
    /// <summary>
    /// Function to get item from the list
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
            private static readonly Type[] Arguments = new Type[] { Type.List(Param), Type.IntT };

            public OverloadOne()
            : base(Arguments, Param)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                ListValue list = (ListValue)arguments[0];
                int index = (int)((IntValue)arguments[1]).Value;

                if (index >= 0 && index < list.Value.Count)
                {
                    context.SetFunctionReturnValue(list.Value[index]);
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
