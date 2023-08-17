using UglyLang.Source.Types;

namespace UglyLang.Source.Values
{
    public class NamespaceValue : Value
    {
        public Dictionary<string,ISymbolValue> Value;

        public NamespaceValue() : base(new NamespaceType())
        {
            Value = new();
        }

        public override bool IsTruthy()
        {
            return true;
        }

        public override Value? To(Types.Type type)
        {
            if (type is StringType)
            {
                return new StringValue("NAMESPACE{" + string.Join(",", Value.Keys) + "}");
            }

            return null;
        }

        public override bool Equals(Value value)
        {
            return value is NamespaceValue nsValue && Value == nsValue.Value;
        }

        protected override bool HasPropertyExtra(string name)
        {
            if (Value.ContainsKey(name)) return true;
            return false;
        }

        protected override Property? GetPropertyExtra(string name)
        {
            if (Value.ContainsKey(name)) return new(name, Value[name], true);
            return null;
        }

        protected override bool SetPropertyExtra(string name, ISymbolValue value)
        {
            return false;
        }
    }
}
