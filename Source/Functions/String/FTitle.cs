using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.String
{
    /// <summary>
    /// Function to return the title-case version of the string
    /// </summary>
    public class FTitle : Function
    {
        public FTitle()
        {
            Overloads.Add(new OverloadOne());
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Types.Type[] Arguments = new Types.Type[] { Types.Type.StringT };

            public OverloadOne()
            : base(Arguments, Types.Type.StringT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                StringValue value = new(string.Join(" ", ((StringValue)arguments[0]).Value.Split(" ").Select(s => s[0].ToString().ToUpper() + s[1..].ToLower())));
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
