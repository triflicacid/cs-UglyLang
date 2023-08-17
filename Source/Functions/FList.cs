using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions
{
    /// <summary>
    /// Function to create a list of said type
    /// </summary>
    public class FList : Function, IDefinedGlobally
    {
        public FList()
        {
            Overloads.Add(new OverloadOne());
            Overloads.Add(new OverloadTwo());
        }

        public string GetDefinedName()
        {
            return "LIST";
        }



        internal class OverloadOne : FunctionOverload
        {
            private static readonly Types.Type[] Arguments = new Types.Type[] { new TypeType() };

            public OverloadOne()
            : base(Arguments, new ListType(new Any()))
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                Types.Type type = ((TypeValue)arguments[0]).Value;
                context.SetFunctionReturnValue(new ListValue(type));
                return Signal.NONE;
            }
        }

        internal class OverloadTwo : FunctionOverload
        {
            private static readonly Types.Type[] Arguments = new Types.Type[] { new TypeType(), Types.Type.IntT };

            public OverloadTwo()
            : base(Arguments, new ListType(new Any()))
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                Types.Type type = ((TypeValue)arguments[0]).Value;
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
                    Value? value = type.ConstructNoArgs(context);
                    if (value == null) return Signal.ERROR;
                    values.Add(value);
                }

                context.SetFunctionReturnValue(new ListValue(type, values));
                return Signal.NONE;
            }
        }
    }
}
