using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.List
{
    /// <summary>
    /// Function to add an item to the list
    /// </summary>
    public class FContains : Function
    {
        private static readonly TypeParameter Param = new("a");

        public FContains()
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
                IntValue value = new(((ListValue)arguments[0]).Contains(arguments[1]));
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
