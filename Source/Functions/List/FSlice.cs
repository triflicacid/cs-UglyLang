using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.List
{
    /// <summary>
    /// Function to return a portion of the list
    /// </summary>
    public class FSlice : Function
    {
        private static readonly Types.Type List = Types.Type.List(new TypeParameter("a"));

        public FSlice()
        {
            Overloads.Add(new OverloadOne());
            Overloads.Add(new OverloadTwo());
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Types.Type[] Arguments = new Types.Type[] { List, Types.Type.IntT };

            public OverloadOne()
            : base(Arguments, List)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                ListValue list = (ListValue)arguments[0];
                int startIndex = (int)((IntValue)arguments[1]).Value;
                ListValue listSlice = new(((ListType)list.Type).Member);

                for (int i = startIndex; i < list.Value.Count; i++)
                {
                    listSlice.Value.Add(list.Value[i]);
                }

                context.SetFunctionReturnValue(listSlice);
                return Signal.NONE;
            }
        }

        internal class OverloadTwo : FunctionOverload
        {
            private static readonly Types.Type[] Arguments = new Types.Type[] { List, Types.Type.IntT, Types.Type.IntT };

            public OverloadTwo()
            : base(Arguments, List)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                ListValue list = (ListValue)arguments[0];
                int startIndex = (int)((IntValue)arguments[1]).Value;
                int endIndex = Math.Min((int)((IntValue)arguments[2]).Value, list.Value.Count);
                ListValue listSlice = new(((ListType)list.Type).Member);

                for (int i = startIndex; i < endIndex; i++)
                {
                    listSlice.Value.Add(list.Value[i]);
                }

                context.SetFunctionReturnValue(listSlice);
                return Signal.NONE;
            }
        }
    }
}
