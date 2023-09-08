namespace UglyLang.Source
{
    public class Variable : ILocatable
    {
        private readonly string Name;
        private object Value;
        public bool IsReadonly = false;
        public int LineNumber = -1;
        public int ColumnNumber = -1;

        public Variable(string name, object value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Clone a Variable (doesn't clone the Value property)
        /// </summary>
        public Variable(Variable other)
        {
            Name = other.Name;
            Value = other.Value;
            IsReadonly = other.IsReadonly;
        }

        /// <summary>
        /// Take all properties from 'other' except the value itself
        /// </summary>
        public Variable(Variable other, object value)
        {
            Name = other.Name;
            Value = value;
            IsReadonly = other.IsReadonly;
        }

        public string GetName()
        {
            return Name;
        }

        public object GetValue()
        {
            return Value;
        }

        public void SetValue(object value)
        {
            Value = value;
        }

        public static Dictionary<string, Variable> CreateDictionary(Variable[] variables)
        {
            Dictionary<string, Variable> d = new();
            foreach (var v in variables)
                d.Add(v.Name, v);
            return d;
        }

        public int GetLineNumber()
        {
            return LineNumber;
        }

        public int GetColumnNumber()
        {
            return ColumnNumber;
        }
    }
}
