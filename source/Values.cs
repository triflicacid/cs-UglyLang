namespace UglyLang.source
{
    abstract public class Value
    {
        public ValueType Type;
    }

    public enum ValueType
    {
        EMPTY,
        INT,
        FLOAT,
        STRING
    }

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

        public static IntValue From(Value value)
        {
            if (value is IntValue ivalue) return new(ivalue.Value);
            if (value is FloatValue fvalue) return new((long) fvalue.Value);
            if (value is StringValue svalue) return new((long) Convert.ToDouble(svalue.Value));
            throw new Exception("Unable to cast: unknown value type passed");
        }
    }

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

        public static FloatValue From(Value value)
        {
            if (value is IntValue ivalue) return new(ivalue.Value);
            if (value is FloatValue fvalue) return new(fvalue.Value);
            if (value is StringValue svalue) return new(Convert.ToDouble(svalue.Value));
            throw new Exception("Unable to cast: unknown value type passed");
        }
    }

    /// <summary>
    /// Value representing a string
    /// </summary>
    public class StringValue : Value
    {
        public string Value;

        public StringValue(string value = "")
        {
            Value = value;
            Type = ValueType.STRING;
        }

        public static StringValue From(Value value)
        {
            if (value is IntValue ivalue) return new(ivalue.Value.ToString());
            if (value is FloatValue fvalue) return new(fvalue.Value.ToString());
            if (value is StringValue svalue) return new(svalue.Value);
            throw new Exception("Unable to cast: unknown value type passed");
        }
    }

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
    }
}
