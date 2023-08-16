﻿using UglyLang.Source.Types;

namespace UglyLang.Source.Values
{
    public class MapValue : Value
    {
        public static readonly string ValueTypeProperty = "ValueType";

        public Dictionary<string, Value> Value;

        public MapValue(Types.Type ValueType) : base(new MapType(ValueType))
        {
            Value = new();
        }

        public override bool IsTruthy()
        {
            return true;
        }

        public override Value? To(Types.Type type)
        {
            if (type is MapType map && map.Equals((MapType)Type)) return this;
            if (type is StringType)
            {
                List<string> Members = new();
                foreach (var pair in Value)
                {
                    Value? s = pair.Value.To(type);
                    if (s == null) throw new InvalidOperationException(string.Format("Error converting {0} to STRING - this cast should be implemented", pair.Value.Type));
                    Members.Add(pair.Key + " => " + ((StringValue)s).Value);
                }

                return new StringValue("{" + string.Join(",", Members) + "}");
            }

            return null;
        }

        public override bool Equals(Value value)
        {
            // TODO: Check entries of each?
            return false;
        }

        protected override bool HasPropertyExtra(string name)
        {
            if (name == ValueTypeProperty) return true;
            return false;
        }

        protected override Property? GetPropertyExtra(string name)
        {
            if (name == ValueTypeProperty) return new Property(ValueTypeProperty, new TypeValue(((MapType)Type).ValueType), true);
            return null;
        }
    }
}
