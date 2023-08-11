using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.String
{
    /// <summary>
    /// Function to to split a string by a seperator into a list
    /// </summary>
    public class FSplit : Function
    {
        private static readonly StringType String = new();

        private static readonly List<UnresolvedType[]> Arguments = new()
        {
            new UnresolvedType[] { ResolvedType.String, ResolvedType.String },
        };

        public FSplit() : base(Arguments, ResolvedType.List(String)) { }

        protected override Signal CallOverload(Context context, int _, List<Value> arguments, TypeParameterCollection c)
        {
            string s = ((StringValue)arguments[0]).Value;
            string sep = ((StringValue)arguments[1]).Value;
            ListValue list = new(String);

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
