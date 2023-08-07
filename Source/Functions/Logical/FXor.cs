﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.Logical
{
    public class FXOr : Function
    {

        private static readonly List<Types.Type[]> ArgumentType = new()
        {
            new Types.Type[] { new Any(), new Any() },
        };

        public FXOr() : base(ArgumentType, new IntType()) { }

        protected override Value CallOverload(Context context, int _, List<Value> arguments)
        {
            bool a = arguments[0].IsTruthy(), b = arguments[1].IsTruthy();
            return new IntValue((a && !b) || (!a && b));
        }
    }
}
