﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UglyLang.Source.Values
{
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

        public override bool IsTruthy()
        {
            return Value.Length > 0;
        }

        public static StringValue From(Value value)
        {
            if (value is IntValue ivalue) return new(ivalue.Value.ToString());
            if (value is FloatValue fvalue) return new(fvalue.Value.ToString());
            if (value is StringValue svalue) return new(svalue.Value);
            if (value is EmptyValue) return new("");
            throw new Exception("Unable to cast: unknown value type passed");
        }

        public override Value To(ValueType type)
        {
            return type switch
            {
                ValueType.ANY => new StringValue(Value),
                ValueType.INT => new IntValue((long)StringToDouble(Value)),
                ValueType.FLOAT => new FloatValue(StringToDouble(Value)),
                ValueType.STRING => new StringValue(Value),
                _ => throw new Exception("Unable to cast: unknown value type passed")
            };
        }

        public StringValue Concat(string str)
        {
            return new StringValue(Value + str);
        }
    }
}
