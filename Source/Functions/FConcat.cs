using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions
{
    /// <summary>
    /// Concatenates two items as strings
    /// </summary>
    public class FConcat : Function, IDefinedGlobally
    {
        public FConcat()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "CONCAT";
        }



        internal class OverloadOne : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.AnyT, Type.AnyT };

            public OverloadOne()
            : base(Arguments, new StringType())
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                StringValue value = new(StringValue.From(arguments[0]).Value + StringValue.From(arguments[1]).Value);
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
