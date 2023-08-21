using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions
{
    /// <summary>
    /// Sleep the current thread for some milliseconds
    /// </summary>
    public class FSleep : Function, IDefinedGlobally
    {
        public FSleep()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "SLEEP";
        }



        internal class OverloadOne : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.IntT };

            public OverloadOne()
            : base(Arguments, Type.EmptyT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                Thread.Sleep((int)((IntValue)arguments[0]).Value);
                return Signal.NONE;
            }
        }
    }
}
