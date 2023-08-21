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
            Overloads.Add(new OverloadTwo());
        }

        public string GetDefinedName()
        {
            return "MAP";
        }


        internal class OverloadOne : FunctionOverload
        {
            public OverloadOne()
            : base(Array.Empty<Type>(), Type.TypeT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                context.SetFunctionReturnValue(new TypeValue(Type.Map(Type.AnyT)));
                return Signal.NONE;
            }
        }

        internal class OverloadTwo : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { new TypeType() };

            public OverloadTwo()
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
