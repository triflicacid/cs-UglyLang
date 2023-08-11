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
    /// Function to return a sliced portion of the string
    /// </summary>
    public class FSlice : Function
    {
        private static readonly List<UnresolvedType[]> Arguments = new()
        {
            new UnresolvedType[] { ResolvedType.String, ResolvedType.Int },
            new UnresolvedType[] { ResolvedType.String, ResolvedType.Int, ResolvedType.Int },
        };

        public FSlice() : base(Arguments, ResolvedType.String) { }

        protected override Signal CallOverload(Context context, int index, List<Value> arguments, TypeParameterCollection c)
        {
            string s = ((StringValue)arguments[0]).Value;
            int startIndex = (int)((IntValue)arguments[1]).Value;
            string substr;

            if (index == 1)
            {
                int endIndex = Math.Max((int)((IntValue)arguments[2]).Value, startIndex);
                substr = s.Substring(startIndex, endIndex - startIndex);
            }
            else
            {
                substr = s.Substring(startIndex);
            }

            context.SetFunctionReturnValue(new StringValue(substr));
            return Signal.NONE;
        }
    }
}
