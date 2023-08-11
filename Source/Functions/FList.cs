using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions
{
    /// <summary>
    /// Function to create a list of said type
    /// </summary>
    public class FList : Function, IDefinedGlobally
    {
        public FList()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "LIST";
        }



        internal class OverloadOne : FunctionOverload
        {
            private readonly static Types.Type[] Arguments = new Types.Type[] { new TypeType() };

            public OverloadOne()
            : base(Arguments, new ListType(new Any()))
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters)
            {
                Types.Type type = ((TypeValue)arguments[0]).Value;
                context.SetFunctionReturnValue(new ListValue(type));
                return Signal.NONE;
            }
        }
    }
}
