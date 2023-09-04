using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.Types
{
    public class FStringConstructor : Function
    {
        public FStringConstructor()
        {
            Overloads.Add(new OverloadOne());
            Overloads.Add(new OverloadTwo());
        }

        internal class OverloadOne : FunctionOverload
        {
            public OverloadOne()
            : base(new Type[] { Type.TypeT }, Type.StringT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                context.SetFunctionReturnValue(new StringValue());
                return Signal.NONE;
            }
        }

        internal class OverloadTwo : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.TypeT, Type.AnyT };

            public OverloadTwo()
            : base(Arguments, Type.StringT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                Value value = arguments[1];
                Value? newValue = value.To(Type.StringT);
                if (newValue == null)
                {
                    context.Error = new(lineNo, colNo, Error.Types.Cast, string.Format("casting {0} to {1}", value.Type, "STRING"));
                    return Signal.ERROR;
                }
                else
                {
                    context.SetFunctionReturnValue(newValue);
                    return Signal.NONE;
                }
            }
        }
    }
}
