﻿using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.List
{
    /// <summary>
    /// Function to reverse said list
    /// </summary>
    public class FReverse : Function
    {
        private static readonly Types.Type List = Types.Type.List(new TypeParameter("a"));

        public FReverse()
        {
            Overloads.Add(new OverloadOne());
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Types.Type[] Arguments = new Types.Type[] { List };

            public OverloadOne()
            : base(Arguments, List)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                ListValue list = (ListValue)arguments[0];
                List<Value> copy = new(list.Value);
                copy.Reverse();
                context.SetFunctionReturnValue(new ListValue(((ListType)list.Type).Member, copy));
                return Signal.NONE;
            }
        }
    }
}
