using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.Types
{
    public class FIntConstructor : Function
    {
        public FIntConstructor()
        {
            Overloads.Add(new OverloadOne());
            Overloads.Add(new OverloadTwo());
        }

        internal class OverloadOne : FunctionOverload
        {
            public OverloadOne()
            : base(new Type[] { Type.TypeT }, Type.IntT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                context.SetFunctionReturnValue(new IntValue());
                return Signal.NONE;
            }
        }

        internal class OverloadTwo : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.TypeT, Type.AnyT };

            public OverloadTwo()
            : base(Arguments, Type.IntT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                Value value = arguments[1];
                Value? newValue = value.To(Type.IntT);
                if (newValue == null)
                {
                    context.Error = new(lineNo, colNo, Error.Types.Cast, string.Format("casting {0} to {1}", value.Type, "INT"));
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
