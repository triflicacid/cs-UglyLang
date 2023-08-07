﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;

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
            Type = new IntType();
        }

        public IntValue(bool value)
        {
            Value = value ? 1 : 0;
            Type = new IntType();
        }

        public override bool IsTruthy()
        {
            return Value != 0;
        }

        public static IntValue From(Value value)
        {
            if (value is IntValue ivalue) return new(ivalue.Value);
            if (value is FloatValue fvalue) return new((long)fvalue.Value);
            if (value is StringValue svalue) return new((long)StringToDouble(svalue.Value));
            throw new InvalidOperationException(value.Type.ToString());
        }

        public override Value To(Types.Type type)
        {
            if (type is Any or IntType) return new IntValue(Value);
            if (type is FloatType) return new FloatValue(Value);
            if (type is StringType) return new StringValue(Value.ToString());
            throw new InvalidOperationException(type.ToString());
        }

        public static IntValue Default()
        {
            return new IntValue(0);
        }
    }
}
