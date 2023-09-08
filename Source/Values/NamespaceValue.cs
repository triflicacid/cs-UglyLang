using UglyLang.Source.Types;

namespace UglyLang.Source.Values
{
    public class NamespaceValue : Value, IVariableContainer
    {
        public Dictionary<string, Variable> Value;

        public NamespaceValue() : base(new NamespaceType())
        {
            Value = new();
        }

        public NamespaceValue(Dictionary<string, Variable> members) : base(new NamespaceType())
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
            if (Value.ContainsKey(name))
                return true;
            return false;
        }

        protected override Variable? GetPropertyExtra(string name)
        {
            if (Value.ContainsKey(name))
                return Value[name];
            return null;
        }

        protected override bool SetPropertyExtra(string name, ISymbolValue value)
        {
            if (Value.ContainsKey(name) && Value[name].GetValue() is Value)
            {
                Value[name].SetValue(value);
                return true;
            }

            return false;
        }

        public override bool AreFunctionsContextual()
        {
            return false;
        }

        public bool HasSymbol(string symbol)
        {
            return Value.ContainsKey(symbol);
        }

        public Variable GetSymbol(string symbol)
        {
            return Value[symbol];
        }

        public void CreateSymbol(Variable value)
        {
            Value.Add(value.GetName(), value);
        }

        public void SetSymbol(string symbol, ISymbolValue value)
        {
            Value[symbol].SetValue(value);
        }
    }
}
