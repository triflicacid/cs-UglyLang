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
    /// Function to return a portion of the list
    /// </summary>
    public class FSlice : Function
    {
        private static readonly UnresolvedType ListType= ResolvedType.List(new TypeParameter("a")); // For convenience of repetition below
        private static readonly List<UnresolvedType[]> Arguments = new()
        {
            new UnresolvedType[] { ListType, ResolvedType.Int }, // startIndex
            new UnresolvedType[] { ListType, ResolvedType.Int, ResolvedType.Int }, // startIndex, endIndex
        };

        public FSlice() : base(Arguments, ListType) { }

        protected override Signal CallOverload(Context context, int index, List<Value> arguments, TypeParameterCollection c)
        {
            ListValue list = (ListValue)arguments[0];
            int startIndex = (int)((IntValue)arguments[1]).Value;
            ListValue listSlice = new(((ListType)list.Type).Member);

            if (index == 1)
            {
                int endIndex = Math.Min((int)((IntValue)arguments[2]).Value, list.Value.Count);

                for (int i = startIndex; i < endIndex; i++)
                {
                    listSlice.Value.Add(list.Value[i]);
                }
            }
            else
            {
                for (int i = startIndex; i < list.Value.Count; i++)
                {
                    listSlice.Value.Add(list.Value[i]);
                }
            }

            context.SetFunctionReturnValue(listSlice);
            return Signal.NONE;
        }
    }
}
