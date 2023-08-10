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
    /// Function to construct a new type *without any arguments*
    /// </summary>
    public class FNew : Function
    {
        private static readonly List<UnresolvedType[]> Arguments = new()
        {
            new UnresolvedType[] { ResolvedType.Type },
        };

        public FNew() : base(Arguments, ResolvedType.Any) { }

        protected override Signal CallOverload(Context context, int _, List<Value> arguments, TypeParameterCollection c)
        {
            Types.Type type = ((TypeValue)arguments[0]).Value;

            // Can the type be constructed?
            if (!type.CanConstruct())
            {
                context.Error = new(0, 0, Error.Types.Type, string.Format("type {0} cannot be constructed", type));
                return Signal.NONE;
            }

            Value? value = type.ConstructNoArgs(context);
            if (value == null)
            {
                context.Error = new(0, 0, Error.Types.Type, string.Format("type constructor {0} requires arguments", type));
            }
            else
            {
                context.SetFunctionReturnValue(value);
            }

            return Signal.NONE;
        }
    }
}
