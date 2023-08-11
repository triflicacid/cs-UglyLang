using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public FSlice()
        {
            Overloads.Add(new OverloadOne());
            Overloads.Add(new OverloadTwo());
        }


        internal class OverloadOne : FunctionOverload
        {
            private readonly static Types.Type[] Arguments = new Types.Type[] { Types.Type.StringT, Types.Type.IntT };

            public OverloadOne()
            : base(Arguments, Types.Type.StringT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters)
            {
                string s = ((StringValue)arguments[0]).Value;
                int startIndex = (int)((IntValue)arguments[1]).Value;
                string substr = s.Substring(startIndex);
                context.SetFunctionReturnValue(new StringValue(substr));
                return Signal.NONE;
            }
        }

        internal class OverloadTwo : FunctionOverload
        {
            private readonly static Types.Type[] Arguments = new Types.Type[] { Types.Type.StringT, Types.Type.IntT, Types.Type.IntT };

            public OverloadTwo()
            : base(Arguments, Types.Type.StringT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters)
            {
                string s = ((StringValue)arguments[0]).Value;
                int startIndex = (int)((IntValue)arguments[1]).Value;
                int endIndex = Math.Max((int)((IntValue)arguments[2]).Value, startIndex);
                string substr = s.Substring(startIndex, endIndex - startIndex);
                context.SetFunctionReturnValue(new StringValue(substr));
                return Signal.NONE;
            }
        }
    }
}
