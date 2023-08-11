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
    /// Returns the type of the argument as a string
    /// </summary>
    public class FType : Function, IDefinedGlobally
    {
        public FType()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "TYPE";
        }



        internal class OverloadOne : FunctionOverload
        {
            private readonly static Types.Type[] Arguments = new Types.Type[] { Types.Type.AnyT };

            public OverloadOne()
            : base(Arguments, Types.Type.TypeT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters)
            {
                TypeValue value = new(arguments[0].Type);
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
