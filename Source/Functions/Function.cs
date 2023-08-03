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
        public readonly List<Values.ValueType[]> ArgumentTypes;
        public readonly Values.ValueType ReturnType;

        public Function(List<Values.ValueType[]> argumentTypes, Values.ValueType returnType)
        {
            ArgumentTypes = argumentTypes;
            ReturnType = returnType;
        }

        /// <summary>
        /// Call the function. Note that the given argument list matches with ONE ArgumentTypes member (this is checked in FuncValue. The stack frames and argument evaluation are handled in SymbolNode).
        /// </summary>
        public abstract Value? Call(Context context, int overloadIndex, List<Value> arguments);
    }
}
