using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.List
{
    /// <summary>
    /// Function to add an item to the list
    /// </summary>
    public class FAdd : Function
    {
        private static readonly TypeParameter TypeParam = new("a"); // For convenience of repetition below

        public FAdd()
        {
            Overloads.Add(new OverloadOne());
            Overloads.Add(new OverloadTwo());
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Types.Type[] Arguments = new Types.Type[] { Types.Type.List(TypeParam), TypeParam };

            public OverloadOne()
            : base(Arguments, Types.Type.EmptyT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters)
            {
                ListValue list = (ListValue)arguments[0];
                list.Value.Add(arguments[1]);
                return Signal.NONE;
            }
        }

        internal class OverloadTwo : FunctionOverload
        {
            private static readonly Types.Type[] Arguments = new Types.Type[] { Types.Type.List(TypeParam), TypeParam, Types.Type.IntT };

            public OverloadTwo()
            : base(Arguments, Types.Type.EmptyT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters)
            {
                ListValue list = (ListValue)arguments[0];
                list.Value.Insert((int)((IntValue)arguments[2]).Value, arguments[1]);
                return Signal.NONE;
            }
        }
    }
}
