using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;

namespace UglyLang.Source.Values
{
    /// <summary>
    /// A value which contains nothing - undefined/null
    /// </summary>
    public class EmptyValue : Value
    {
        public EmptyValue()
        {
            Type = new None();
        }

        public static EmptyValue From(Value value)
        {
            throw new NotSupportedException();
        }

        public override bool IsTruthy()
        {
            return false;
        }

        public override Value To(Types.Type type)
        {
            if (type is Any or None) return this;
            if (type is IntType) return IntValue.Default();
            if (type is FloatType) return FloatValue.Default();
            if (type is StringType) return StringValue.Default();
            throw new InvalidOperationException(type.ToString());
        }
    }
}
