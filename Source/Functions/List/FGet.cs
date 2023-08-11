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
        private static readonly TypeParameter TypeParam = new("a"); // For convenience of repetition below
        private static readonly List<UnresolvedType[]> Arguments = new()
        {
            new UnresolvedType[] { ResolvedType.List(TypeParam), ResolvedType.Int },
        };

        public FGet() : base(Arguments, new ResolvedType(TypeParam)) { }

        protected override Signal CallOverload(Context context, int _, List<Value> arguments, TypeParameterCollection c)
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
