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
    /// Concatenates two items as strings
    /// </summary>
    public class FConcat : Function, IDefinedGlobally
    {
        public FConcat()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "CONCAT";
        }



        internal class OverloadOne : FunctionOverload
        {
            private readonly static Types.Type[] Arguments = new Types.Type[] { new Any(), new Any() };

            public OverloadOne()
            : base(Arguments, new StringType())
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters)
            {
                StringValue value = new(StringValue.From(arguments[0]).Value + StringValue.From(arguments[1]).Value);
                context.SetFunctionReturnValue(value);
                return Signal.NONE;
            }
        }
    }
}
