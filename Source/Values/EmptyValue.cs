using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UglyLang.Source.Values
{
    /// <summary>
    /// A value which contains nothing - undefined/null
    /// </summary>
    public class EmptyValue : Value
    {
        public EmptyValue()
        {
            Type = ValueType.EMPTY;
        }

        public static EmptyValue From(Value value)
        {
            throw new NotSupportedException();
        }

        public override bool IsTruthy()
        {
            return false;
        }

        public override Value To(ValueType type)
        {
            return type switch
            {
                ValueType.INT => new IntValue(0),
                ValueType.FLOAT => new FloatValue(0),
                ValueType.STRING => new StringValue(""),
                _ => throw new Exception("Unable to cast: unknown value type passed")
            };
        }
    }
}
