using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.Types
{
    /// <summary>
    /// Function to create a list of said type
    /// </summary>
    public class FListConstructor : Function
    {
        public FListConstructor()
        {
            Overloads.Add(new OverloadOne());
            Overloads.Add(new OverloadTwo());
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.TypeT };

            public OverloadOne()
            : base(Arguments, new ListType(new Any()))
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                ListType type = (ListType)((TypeValue)arguments[0]).Value;
                context.SetFunctionReturnValue(new ListValue(type.Member));
                return Signal.NONE;
            }
        }

        internal class OverloadTwo : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.TypeT, Type.IntT };

            public OverloadTwo()
            : base(Arguments, Type.List(Type.AnyT))
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                ListType type = (ListType)((TypeValue)arguments[0]).Value;
                int length = (int)((IntValue)arguments[1]).Value;

                // Can the type be constructed?
                if (!type.CanConstruct())
                {
                    context.Error = new(0, 0, Error.Types.Type, string.Format("type {0} cannot be constructed", type));
                    return Signal.ERROR;
                }

                // Initialise list of given length
                List<Value> values = new();
                for (int i = 0; i < length; i++)
                {
                    values.Add(new EmptyValue(type.Member));
                }

                context.SetFunctionReturnValue(new ListValue(type.Member, values));
                return Signal.NONE;
            }
        }
    }
}
