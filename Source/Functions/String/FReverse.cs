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
    /// Function to return the reversed string
    /// </summary>
    public class FReverse : Function
    {
        public FReverse()
        {
            Overloads.Add(new OverloadOne());
        }


        internal class OverloadOne : FunctionOverload
        {
            private readonly static Types.Type[] Arguments = new Types.Type[] { Types.Type.StringT };

            public OverloadOne()
            : base(Arguments, Types.Type.StringT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters)
            {
                StringValue value = new(string.Join("", ((StringValue)arguments[0]).Value.Reverse().ToArray()));
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
