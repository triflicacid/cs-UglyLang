﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UglyLang.Source.Values
{
    public enum ValueType
    {
        EMPTY,
        ANY,
        INT,
        FLOAT,
        STRING,
        FUNCTION,
    }

    abstract public class Value
    {
        public ValueType Type;

        public static ValueType? TypeFromString(string str)
        {
            return str switch
            {
                "INTEGER" => ValueType.INT,
                "FLOAT" => ValueType.FLOAT,
                "STRING" => ValueType.STRING,
                _ => null,
            };
        }

        public abstract Value To(ValueType type);

        public abstract bool IsTruthy();

        /// <summary>
        /// Does type2 match with type1?
        /// </summary>
        public static bool Match(ValueType type1, ValueType type2)
        {
            if (type1 == ValueType.ANY || type2 == ValueType.ANY) return true;
            if (type1 == type2) return true;
            if (type1 == ValueType.INT && type2 == ValueType.FLOAT) return true;
            return false;
        }
    }
}