using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions
{
    /// <summary>
    /// Function to construct a new type *without any arguments*
    /// </summary>
    public class FNew : Function, IDefinedGlobally
    {
        public FNew()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "NEW";
        }



        internal class OverloadOne : FunctionOverload
        {
            private static readonly Types.Type[] Arguments = new Types.Type[] { new TypeType() };

            public OverloadOne()
            : base(Arguments, new Any())
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters)
            {
                Types.Type type = ((TypeValue)arguments[0]).Value;

                // Can the type be constructed?
                if (!type.CanConstruct())
                {
                    context.Error = new(0, 0, Error.Types.Type, string.Format("type {0} cannot be constructed", type));
                    return Signal.ERROR;
                }

                Value? value = type.ConstructNoArgs(context);
                if (value == null)
                {
                    context.Error = new(0, 0, Error.Types.Type, string.Format("type constructor {0} requires arguments", type));
                    return Signal.ERROR;
                }
                else
                {
                    context.SetFunctionReturnValue(value);
                }

                return Signal.NONE;
            }
        }
    }
}
