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
    /// Function to get item from the list
    /// </summary>
    public class FGet : Function
    {
        private static readonly TypeParameter Param = new("a");

        public FGet()
        {
            Overloads.Add(new OverloadOne());
        }


        internal class OverloadOne : FunctionOverload
        {
            private readonly static Types.Type[] Arguments = new Types.Type[] { Types.Type.List(Param), Types.Type.IntT };

            public OverloadOne()
            : base(Arguments, Param)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters)
            {
                ListValue list = (ListValue)arguments[0];
                int index = (int)((IntValue)arguments[1]).Value;

                if (index >= 0 && index < list.Value.Count)
                {
                    context.SetFunctionReturnValue(list.Value[index]);
                    return Signal.NONE;
                }
                else
                {
                    context.Error = new(0, 0, Error.Types.General, string.Format("index {0} is out of bounds for {1}", index, list.Type));
                    return Signal.ERROR;
                }
            }
        }
    }
}
