using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.Maths
{
    /// <summary>
    /// Return the successor of the given integer
    /// </summary>
    public class FSucc : Function, IDefinedGlobally
    {
        public FSucc()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "SUCC";
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.IntT };

            public OverloadOne()
            : base(Arguments, Type.IntT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                IntValue value = new(((IntValue)arguments[0]).Value + 1);
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
