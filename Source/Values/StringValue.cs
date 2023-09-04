using UglyLang.Source.Types;

namespace UglyLang.Source.Values
{
    /// <summary>
    /// Value representing a string
    /// </summary>
    public class StringValue : Value
    {
        public string Value;

        public StringValue(string value = "") : base(Types.Type.StringT)
        {
            Value = value;
        }

        public override bool IsTruthy()
        {
            return Value.Length > 0;
        }

        public static StringValue From(Value value)
        {
            if (value is IntValue ivalue)
                return new(ivalue.Value.ToString());
            if (value is FloatValue fvalue)
                return new(fvalue.Value.ToString());
            if (value is StringValue svalue)
                return new(svalue.Value);

            Value? v = value.To(Types.Type.StringT);
            return v == null ? throw new InvalidOperationException() : (StringValue)v;
        }

        public override Value? To(Types.Type type)
        {
            if (type is Any or StringType)
                return new StringValue(Value);
            if (type is IntType)
                return new IntValue((long)StringToDouble(Value));
            if (type is FloatType)
                return new FloatValue(StringToDouble(Value));
            return null;
        }

        public override bool Equals(Value value)
        {
            return value is StringValue s && Value == s.Value;
        }

        public static StringValue Default()
        {
            return new StringValue("");
        }
    }
}
