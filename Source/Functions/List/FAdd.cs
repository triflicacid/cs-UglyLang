﻿using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

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
            private static readonly Type[] Arguments = new Type[] { Type.List(TypeParam), TypeParam };

            public OverloadOne()
            : base(Arguments, Type.EmptyT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                ListValue list = (ListValue)arguments[0];
                list.Value.Add(arguments[1]);
                return Signal.NONE;
            }
        }

        internal class OverloadTwo : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.List(TypeParam), TypeParam, Type.IntT };

            public OverloadTwo()
            : base(Arguments, Type.EmptyT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                ListValue list = (ListValue)arguments[0];
                list.Value.Insert((int)((IntValue)arguments[2]).Value, arguments[1]);
                return Signal.NONE;
            }
        }
    }
}
