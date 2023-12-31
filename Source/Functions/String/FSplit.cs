﻿using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

namespace UglyLang.Source.Functions.String
{
    /// <summary>
    /// Function to to split a string by a seperator into a list
    /// </summary>
    public class FSplit : Function
    {
        public FSplit()
        {
            Overloads.Add(new OverloadOne());
        }


        internal class OverloadOne : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { Type.StringT, Type.StringT };

            public OverloadOne()
            : base(Arguments, Type.StringT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                string s = ((StringValue)arguments[0]).Value;
                string sep = ((StringValue)arguments[1]).Value;
                ListValue list = new(Type.StringT);

                if (sep.Length == 0)
                {
                    foreach (char ch in s)
                    {
                        list.Value.Add(new StringValue(ch.ToString()));
                    }
                }
                else
                {
                    foreach (string seg in s.Split(sep))
                    {
                        list.Value.Add(new StringValue(seg));
                    }
                }

                context.SetFunctionReturnValue(list);
                return Signal.NONE;
            }
        }
    }
}
