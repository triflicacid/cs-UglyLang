using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.List
{
    /// <summary>
    /// Function to remove any item from a list
    /// </summary>
    public class FRemove : Function
    {
        private static readonly TypeParameter Param = new("a");

        public FRemove()
        {
            Overloads.Add(new OverloadOne());
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.List(Param), Param };

            public OverloadOne()
            : base(Arguments, Type.IntT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                ListValue list = (ListValue)arguments[0];
                IntValue value = new(list.Remove(arguments[1]));
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
