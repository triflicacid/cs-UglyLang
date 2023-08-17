﻿using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.List
{
    /// <summary>
    /// Function to remove an item at the given index from the list
    /// </summary>
    public class FRemoveAt : Function
    {
        private static readonly TypeParameter Param = new("a");

        public FRemoveAt()
        {
            Overloads.Add(new OverloadOne());
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Types.Type[] Arguments = new Types.Type[] { Types.Type.List(Param), Types.Type.IntT };

            public OverloadOne()
            : base(Arguments, Types.Type.IntT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                ListValue list = (ListValue)arguments[0];
                IntValue value = new(list.RemoveAt((int)((IntValue)arguments[1]).Value));
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
