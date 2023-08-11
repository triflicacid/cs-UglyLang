﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.String
{
    /// <summary>
    /// Function to check whether the string contains the given substring
    /// </summary>
    public class FContains : Function
    {
        public FContains()
        {
            Overloads.Add(new OverloadOne());
        }


        internal class OverloadOne : FunctionOverload
        {
            private readonly static Types.Type[] Arguments = new Types.Type[] { Types.Type.StringT, Types.Type.StringT};

            public OverloadOne()
            : base(Arguments, Types.Type.IntT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters)
            {
                IntValue value = new(((StringValue)arguments[0]).Value.Contains(((StringValue)arguments[1]).Value));
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
