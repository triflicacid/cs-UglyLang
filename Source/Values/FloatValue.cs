﻿using UglyLang.Source.Types;

namespace UglyLang.Source.Values
{
    /// <summary>
    /// Value representing a decimal number
    /// </summary>
    public class FloatValue : Value
    {
        public double Value;

        public FloatValue(double value = 0) : base(new FloatType())
        {
            Value = value;
        }

        public override bool IsTruthy()
        {
            return Value != 0;
        }

        public static FloatValue From(Value value)
        {
            if (value is IntValue ivalue)
                return new(ivalue.Value);
            if (value is FloatValue fvalue)
                return new(fvalue.Value);
            if (value is StringValue svalue)
                return new(StringToDouble(svalue.Value));
            throw new InvalidOperationException(value.Type.ToString());
        }

        public override Value? To(Types.Type type)
        {
            if (type is Any or FloatType)
                return new FloatValue(Value);
            if (type is IntType)
                return new IntValue((long)Value);
            if (type is StringType)
                return new StringValue(Value.ToString());
            return null;
        }

        public static FloatValue Default()
        {
            return new FloatValue(0);
        }

        public override bool Equals(Value value)
        {
            return (value is FloatValue f && f.Value == Value) || (value is IntValue i && i.Value == Value);
        }
    }
}
