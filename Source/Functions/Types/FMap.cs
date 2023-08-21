using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.Types
{
    /// <summary>
    /// Function to create a map of said type
    /// </summary>
    public class FMap : Function, IDefinedGlobally
    {
        public FMap()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "MAP";
        }



        internal class OverloadOne : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { new TypeType() };

            public OverloadOne()
            : base(Arguments, new MapType(new Any()))
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                Type type = ((TypeValue)arguments[0]).Value;
                context.SetFunctionReturnValue(new MapValue(type));
                return Signal.NONE;
            }
        }
    }
}
