using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.String
{
    /// <summary>
    /// Function to return a sliced portion of the string
    /// </summary>
    public class FSlice : Function
    {
        public FSlice()
        {
            Overloads.Add(new OverloadOne());
            Overloads.Add(new OverloadTwo());
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.StringT, Type.IntT };

            public OverloadOne()
            : base(Arguments, Type.StringT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                string s = ((StringValue)arguments[0]).Value;
                int startIndex = (int)((IntValue)arguments[1]).Value;
                string substr = s[startIndex..];
                context.SetFunctionReturnValue(new StringValue(substr));
                return Signal.NONE;
            }
        }

        internal class OverloadTwo : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.StringT, Type.IntT, Type.IntT };

            public OverloadTwo()
            : base(Arguments, Type.StringT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                string s = ((StringValue)arguments[0]).Value;
                int startIndex = (int)((IntValue)arguments[1]).Value;
                int endIndex = Math.Max((int)((IntValue)arguments[2]).Value, startIndex);
                string substr = s[startIndex..endIndex];
                context.SetFunctionReturnValue(new StringValue(substr));
                return Signal.NONE;
            }
        }
    }
}
