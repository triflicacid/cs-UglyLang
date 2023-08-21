using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.String
{
    /// <summary>
    /// Function to return the reversed string
    /// </summary>
    public class FReverse : Function
    {
        public FReverse()
        {
            Overloads.Add(new OverloadOne());
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.StringT };

            public OverloadOne()
            : base(Arguments, Type.StringT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                StringValue value = new(string.Join("", ((StringValue)arguments[0]).Value.Reverse().ToArray()));
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
