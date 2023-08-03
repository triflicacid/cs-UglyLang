﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.Logical
{
    public class FOr : Function
    {

        private static readonly Values.ValueType[][] ArgumentType = new Values.ValueType[][]
        {
            new Values.ValueType[] { Values.ValueType.ANY, Values.ValueType.ANY },
        };

        public FOr() : base(ArgumentType, Values.ValueType.INT) { }

        public override Value Call(Context context, List<Value> arguments)
        {
            Value a = arguments[0], b = arguments[1];
            return new IntValue(a.IsTruthy() || b.IsTruthy());
        }
    }
}