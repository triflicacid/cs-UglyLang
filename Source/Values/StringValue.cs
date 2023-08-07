﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;

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
            Type = new StringType();
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
            throw new InvalidOperationException(value.Type.ToString());
        }

        public override Value To(Types.Type type)
        {
            if (type is Any or StringType) return new StringValue(Value);
            if (type is IntType) return new IntValue((long)StringToDouble(Value));
            if (type is FloatType) return new FloatValue(StringToDouble(Value));
            throw new InvalidOperationException(type.ToString());
        }

        public StringValue Concat(string str)
        {
            return new StringValue(Value + str);
        }

        public static StringValue Default()
        {
            return new StringValue("");
        }
    }
}
