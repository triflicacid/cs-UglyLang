using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UglyLang.Source.Values
{
    /// <summary>
    /// Value representing a decimal number
    /// </summary>
    public class FloatValue : Value
    {
        public double Value;

        public FloatValue(double value = 0)
        {
            Value = value;
            Type = ValueType.FLOAT;
        }

        public override bool IsTruthy()
        {
            return Value != 0;
        }

        public static FloatValue From(Value value)
        {
            if (value is IntValue ivalue) return new(ivalue.Value);
            if (value is FloatValue fvalue) return new(fvalue.Value);
            if (value is StringValue svalue) return new(Convert.ToDouble(svalue.Value));
            throw new Exception("Unable to cast: unknown value type passed");
        }

        public override Value To(ValueType type)
        {
            return type switch
            {
                ValueType.INT => new IntValue((long)Value),
                ValueType.FLOAT => new FloatValue(Value),
                ValueType.STRING => new StringValue(Value.ToString()),
                _ => throw new Exception("Unable to cast: unknown value type passed")
            };
        }
    }
}
