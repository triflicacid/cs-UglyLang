﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;

namespace UglyLang.Source.Values
{
    public class ListValue : Value
    {
        public static readonly string MemberTypeProperty = "MemberType";

        public List<Value> Value;

        public ListValue(Types.Type MemberType) : base(new ListType(MemberType))
        {
            Value = new();
        }

        public override bool IsTruthy()
        {
            return Value.Count > 0;
        }

        public override Value? To(Types.Type type)
        {
            if (type is ListType list && Type.Equals(list)) return this;
            if (type is StringType)
            {
                List<string> Members = new();
                foreach (Value member in Value)
                {
                    Value? s = member.To(type);
                    if (s == null) throw new InvalidOperationException(string.Format("Error converting {0} to STRING - this cast should be implemented", member.Type));
                    Members.Add(((StringValue)s).Value);
                }

                return new StringValue("{" + string.Join(",", Members) + "}");
            }

            return null;
        }

        public override bool Equals(Value value)
        {
            if (value is ListValue list && list.Value.Count == Value.Count)
            {
                for (int i = 0; i < Value.Count; i++)
                {
                    if (!Value[i].Equals(list.Value[i])) return false;
                }
                return true;
            }

            return false;
        }

        public bool Contains(Value value)
        {
            for (int i = 0; i < Value.Count; i++)
            {
                if (Value[i].Equals(value)) return true;
            }

            return false;
        }

        public int IndexOf(Value value)
        {
            for (int i = 0; i < Value.Count; i++)
            {
                if (Value[i].Equals(value)) return i;
            }

            return -1;
        }

        public bool Remove(Value value)
        {
            bool removed = false;
            for (int i = Value.Count - 1; i >= 0; i--)
            {
                if (Value[i].Equals(value))
                {
                    Value.RemoveAt(i);
                    removed = true;
                }
            }

            return removed;
        }

        public bool RemoveAt(int index)
        {
            if (index >= 0 && index < Value.Count)
            {
                Value.RemoveAt(index);
                return true;
            }

            return false;
        }

        protected override bool HasPropertyExtra(string name)
        {
            if (name == MemberTypeProperty) return true;
            if (double.TryParse(name, out double n) && n >= 0 && n < Value.Count)
            {
                return true;
            }

            return false;
        }

        protected override Property? GetPropertyExtra(string name)
        {
            if (name == MemberTypeProperty) return new Property(MemberTypeProperty, new TypeValue(((ListType)Type).Member), true);
            if (double.TryParse(name, out double n) && n >= 0 && n < Value.Count)
            {
                return new Property(name, Value[(int)n]);
            }

            return null;
        }

        protected override bool SetPropertyExtra(string name, ISymbolValue value)
        {
            if (double.TryParse(name, out double n) && n >= 0 && n < Value.Count)
            {
                Value[(int)n] = (Value)value; // Casting is already taken care of
                return true;
            }

            return false;
        }
    }
}