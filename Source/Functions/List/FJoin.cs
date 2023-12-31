﻿using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.List
{
    /// <summary>
    /// Function to join items by a seperator
    /// </summary>
    public class FJoin : Function
    {
        private static readonly TypeParameter Param = new("a");

        public FJoin()
        {
            Overloads.Add(new OverloadOne());
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.List(Param), Type.StringT };

            public OverloadOne()
            : base(Arguments, Type.StringT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                string glue = ((StringValue)arguments[1]).Value;
                string result = "";
                foreach (Value member in ((ListValue)arguments[0]).Value)
                {
                    Value? stringMember = member.To(new StringType());
                    if (stringMember == null)
                    {
                        context.Error = new(0, 0, Error.Types.Cast, string.Format("casting {0} to STRING", member.Type));
                        return Signal.ERROR;
                    }
                    else
                    {
                        result += glue + ((StringValue)stringMember).Value;
                    }
                }

                result = result.Remove(0, 1); // Remove leading glue character
                context.SetFunctionReturnValue(new StringValue(result));
                return Signal.NONE;
            }
        }
    }
}
