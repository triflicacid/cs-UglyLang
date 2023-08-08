using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;

namespace UglyLang.Source.Values
{
    public class ListValue : Value
    {
        public List<Value> Value;

        public ListValue(Types.Type MemberType)
        {
            Type = new ListType(MemberType);
            Value = new();
        }

        public override bool IsTruthy()
        {
            return Value.Count > 0;
        }

        public override Value? To(Types.Type type)
        {
            if (type is StringType) return new StringValue("{" + string.Join(",", Value.Select(v => ((StringValue)v.To(type)).Value)) + "}");
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

        protected override bool HasPropertyExtra(string name)
        {
            return name.All(c => char.IsDigit(c));
        }

        protected override ISymbolValue? GetPropertyExtra(string name)
        {
            if (double.TryParse(name, out double n))
            {
                if (n >= 0 && n < Value.Count)
                {
                    return Value[(int) n];
                }
                else
                {
                    return new EmptyValue();
                }
            }

            return null;
        }
    }
}
