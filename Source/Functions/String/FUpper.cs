using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.String
{
    /// <summary>
    /// Function to return the upper-case version of the string
    /// </summary>
    public class FUpper : Function
    {
        public FUpper()
        {
            Overloads.Add(new OverloadOne());
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Types.Type[] Arguments = new Types.Type[] { Types.Type.StringT };

            public OverloadOne()
            : base(Arguments, Types.Type.StringT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters)
            {
                StringValue value = new(((StringValue)arguments[0]).Value.ToUpper());
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
