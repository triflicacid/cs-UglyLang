﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions.List
{
    /// <summary>
    /// Function to join items by a seperator
    /// </summary>
    public class FJoin : Function
    {
        private static readonly List<UnresolvedType[]> Arguments = new()
        {
            new UnresolvedType[] { ResolvedType.List(new TypeParameter("a")), ResolvedType.String },
        };

        public FJoin() : base(Arguments, ResolvedType.String) { }

        protected override Signal CallOverload(Context context, int _, List<Value> arguments, TypeParameterCollection c)
        {
            string glue = ((StringValue)arguments[1]).Value;
            string result = "";
            foreach (Value member in ((ListValue)arguments[0]).Value)
            {
                Value? stringMember = member.To(new StringType());
                if (stringMember == null)
                {
                    context.Error = new(0, 0, Error.Types.Cast, string.Format("casting {0} to STRING", member.Type));
                    return Signal.ERROR;
                }
                else
                {
                    result += glue + ((StringValue)stringMember).Value;
                }
            }

            result = result.Remove(0, 1); // Remove leading glue character
            context.SetFunctionReturnValue(new StringValue(result));
            return Signal.NONE;
        }
    }
}