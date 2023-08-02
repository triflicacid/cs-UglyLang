using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions
{
    public abstract class Function
    {
        public Values.ValueType[][] ArgumentTypes;
        public Values.ValueType ReturnType;

        public Function(Values.ValueType[][] argumentTypes, Values.ValueType returnType)
        {
            ArgumentTypes = argumentTypes;
            ReturnType = returnType;
        }

        /// <summary>
        /// Call the function. Note that the given argument list matches with ONE ArgumentTypes member (this is checked in FuncValue).
        /// </summary>
        public abstract Value Call(Context context, List<Value> arguments);
    }
}
