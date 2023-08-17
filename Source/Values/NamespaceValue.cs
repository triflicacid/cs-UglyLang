using UglyLang.Source.Types;

namespace UglyLang.Source.Values
{
    public class NamespaceValue : Value, ISymbolContainer
    {
        public Dictionary<string, ISymbolValue> Value;

        public NamespaceValue() : base(new NamespaceType())
        {
            Value = new();
        }

        public NamespaceValue(Dictionary<string, ISymbolValue> members) : base(new NamespaceType())
        {
            Value = members;
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
            if (Value.ContainsKey(name)) return new(name, Value[name]);
            return null;
        }

        protected override bool SetPropertyExtra(string name, ISymbolValue value)
        {
            if (Value.ContainsKey(name) && Value[name] is Value)
            {
                Value[name] = value;
                return true;
            }

            return false;
        }

        public bool HasSymbol(string symbol)
        {
            return Value.ContainsKey(symbol);
        }

        public ISymbolValue GetSymbol(string symbol)
        {
            return Value[symbol];
        }

        public void CreateSymbol(string symbol, ISymbolValue value)
        {
            Value.Add(symbol, value);
        }

        public void SetSymbol(string symbol, ISymbolValue value)
        {
            Value[symbol] = value;
        }

        public override bool AreFunctionsContextual()
        {
            return false;
        }
    }
}
