using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UglyLang.Source.Values
{
    /// <summary>
    /// Value representing an integer
    /// </summary>
    public class IntValue : Value
    {
        public long Value;

        public IntValue(long value = 0)
        {
            Value = value;
            Type = ValueType.INT;
        }

        public IntValue(bool value)
        {
            Value = value ? 1 : 0;
            Type = ValueType.INT;
        }

        public static IntValue From(Value value)
        {
            if (value is IntValue ivalue) return new(ivalue.Value);
            if (value is FloatValue fvalue) return new((long)fvalue.Value);
            if (value is StringValue svalue) return new((long)Convert.ToDouble(svalue.Value));
            throw new Exception("Unable to cast: unknown value type passed");
        }

        public override Value To(ValueType type)
        {
            return type switch
            {
                ValueType.INT => new IntValue(Value),
                ValueType.FLOAT => new FloatValue(Value),
                ValueType.STRING => new StringValue(Value.ToString()),
                _ => throw new Exception("Unable to cast: unknown value type passed")
            };
        }
    }
}
