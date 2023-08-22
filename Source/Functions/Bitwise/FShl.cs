using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.Bitwise
{
    /// <summary>
    /// Function to shift an integer left by one or more places
    /// </summary>
    public class FShl : Function, IDefinedGlobally
    {
        public FShl()
        {
            Overloads.Add(new OverloadOne());
            Overloads.Add(new OverloadTwo());
        }

        public string GetDefinedName()
        {
            return "SHL";
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.IntT };

            public OverloadOne()
            : base(Arguments, Type.IntT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                IntValue a = (IntValue)arguments[0];
                IntValue value = new(a.Value << 1);
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }

        internal class OverloadTwo : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.IntT, Type.IntT };

            public OverloadTwo()
            : base(Arguments, Type.IntT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                IntValue a = (IntValue)arguments[0], b = (IntValue)arguments[1];
                IntValue value = new(a.Value << (int)b.Value);
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
