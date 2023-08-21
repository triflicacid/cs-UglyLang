using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.Types
{
    /// <summary>
    /// Returns the type of the argument as a string
    /// </summary>
    public class FType : Function, IDefinedGlobally
    {
        public FType()
        {
            Overloads.Add(new OverloadOne());
            Overloads.Add(new OverloadTwo());
        }

        public string GetDefinedName()
        {
            return "TYPE";
        }


        internal class OverloadOne : FunctionOverload
        {
            public OverloadOne()
            : base(Array.Empty<Type>(), Type.TypeT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                context.SetFunctionReturnValue(new TypeValue(Type.TypeT));
                return Signal.NONE;
            }
        }

        internal class OverloadTwo : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.AnyT };

            public OverloadTwo()
            : base(Arguments, Type.TypeT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                TypeValue value = new(arguments[0].Type);
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
