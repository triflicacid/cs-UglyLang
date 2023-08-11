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
    /// Function to remove any item from a list
    /// </summary>
    public class FRemove : Function
    {
        private static readonly TypeParameter Param = new("a");

        public FRemove()
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
                ListValue list = (ListValue)arguments[0];
                IntValue value = new(list.Remove(arguments[1]));
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
