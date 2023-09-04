using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.Types
{
    public class FMapConstructor : Function
    {
        public FMapConstructor()
        {
            Overloads.Add(new OverloadOne());
        }

        internal class OverloadOne : FunctionOverload
        {
            public OverloadOne()
            : base(new Type[] { Type.TypeT }, Type.Map(Type.AnyT))
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                MapType type = (MapType)((TypeValue)arguments[0]).Value;
                context.SetFunctionReturnValue(new MapValue(type.ValueType));
                return Signal.NONE;
            }
        }
    }
}
