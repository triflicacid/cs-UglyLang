using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.Types
{
    public class FFloat : Function, IDefinedGlobally
    {
        public FFloat()
        {
            Overloads.Add(new OverloadOne());
            Overloads.Add(new OverloadTwo());
        }

        public string GetDefinedName()
        {
            return "FLOAT";
        }



        internal class OverloadOne : FunctionOverload
        {
            public OverloadOne()
            : base(Array.Empty<Type>(), Type.TypeT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                context.SetFunctionReturnValue(new TypeValue(Type.FloatT));
                return Signal.NONE;
            }
        }

        internal class OverloadTwo : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.AnyT };

            public OverloadTwo()
            : base(Arguments, Type.FloatT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                Value value = arguments[0];
                Value? newValue = value.To(Type.FloatT);
                if (newValue == null)
                {
                    context.Error = new(lineNo, colNo, Error.Types.Cast, string.Format("casting {0} to {1}", value.Type, "FLOAT"));
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
