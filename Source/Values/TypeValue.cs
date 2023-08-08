using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;

namespace UglyLang.Source.Values
{
    /// <summary>
    /// Value containing a type
    /// </summary>
    public class TypeValue : Value
    {
        public readonly Types.Type Value;

        public TypeValue(Types.Type type)
        {
            Value = type;
            Type = new TypeType();
        }

        public override bool IsTruthy()
        {
            return true;
        }

        public static TypeValue From(Value value)
        {
            throw new InvalidOperationException(value.Type.ToString());
        }

        public override Value To(Types.Type type)
        {
            if (type is Any) return new TypeValue(Value);
            if (type is StringType) return new StringValue(Value.ToString());
            throw new InvalidOperationException(type.ToString());
        }

        public override bool Equals(Value value)
        {
            return value is TypeValue t && Value.Equals(t.Value);
        }
    }
}
