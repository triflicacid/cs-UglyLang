﻿using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.String
{
    /// <summary>
    /// Function to return the upper-case version of the string
    /// </summary>
    public class FUpper : Function
    {
        public FUpper()
        {
            Overloads.Add(new OverloadOne());
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.StringT };

            public OverloadOne()
            : base(Arguments, Type.StringT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                StringValue value = new(((StringValue)arguments[0]).Value.ToUpper());
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
