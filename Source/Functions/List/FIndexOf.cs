﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.List
{
    /// <summary>
    /// Function to find the index of an item in the list
    /// </summary>
    public class FIndexOf : Function
    {
        private static readonly TypeParameter Param = new("a");

        public FIndexOf()
        {
            Overloads.Add(new OverloadOne());
        }


        internal class OverloadOne : FunctionOverload
        {
            private readonly static Types.Type[] Arguments = new Types.Type[] { Types.Type.List(Param), Param };

            public OverloadOne()
            : base(Arguments, Types.Type.IntT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters)
            {
                IntValue value = new(((ListValue)arguments[0]).IndexOf(arguments[1]));
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
