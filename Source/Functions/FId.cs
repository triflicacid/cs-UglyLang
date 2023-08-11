using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions
{
    public class FId : Function, IDefinedGlobally
    {
        public FId()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "ID";
        }



        internal class OverloadOne : FunctionOverload
        {
            private readonly static Types.Type[] Arguments = new Types.Type[] { new TypeParameter("a") };

            public OverloadOne()
            : base(Arguments, new TypeParameter("a"))
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters)
            {
                context.SetFunctionReturnValue(arguments[0]);
                return Signal.NONE;
            }
        }
    }
}
