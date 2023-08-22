using UglyLang.Source.Functions;
using UglyLang.Source.Types;

namespace UglyLang.Source.Values
{
    public abstract class Value : ISymbolValue
    {
        public Types.Type Type;

        public Value(Types.Type type)
        {
            Type = type;
        }



        /// <summary>
        /// Cast this value to the given type, or return null.
        /// </summary>
        public abstract Value? To(Types.Type type);

        /// <summary>
        /// Return whether or not this instance of the current type can be considered as truthy
        /// </summary>
        public abstract bool IsTruthy();

        /// <summary>
        /// Determines whether or not returned functions from GetProperty are wrapped in a FunctionContext wrapper. By default, this returns true.
        /// </summary>
        public virtual bool AreFunctionsContextual()
        {
            return true;
        }

        public bool HasProperty(string name)
        {
            return Type.GetProperties().ContainsKey(name) || HasPropertyExtra(name);
        }

        protected virtual bool HasPropertyExtra(string name)
        {
            return false;
        }

        public Property GetProperty(string name)
        {
            if (!HasProperty(name))
                throw new InvalidOperationException(name);

            Property? prop = Type.GetProperties().ContainsKey(name) ? Type.GetProperties()[name] : GetPropertyExtra(name);
            if (prop == null)
                throw new NullReferenceException();
            if (AreFunctionsContextual() && prop.GetValue() is Function func)
            {
                prop.SetValue(new FunctionContext(func, this));
            }

            return prop;
        }

        protected virtual Property? GetPropertyExtra(string name)
        {
            return null;
        }

        public bool SetProperty(string name, ISymbolValue value)
        {
            // Insert into a function context?
            if (value is Function func)
            {
                value = new FunctionContext(func, this);
            }

            var properties = Type.GetProperties();
            if (properties.ContainsKey(name))
            {
                return properties[name].SetValue(value);
            }
            else
            {
                return SetPropertyExtra(name, value);
            }
        }

        protected virtual bool SetPropertyExtra(string name, ISymbolValue value)
        {
            return false;
        }

        /// <summary>
        /// Convert.ToDouble but fallback to 0 on error
        /// </summary>
        public static double StringToDouble(string str)
        {
            try
            {
                return Convert.ToDouble(str);
            }
            catch
            {
                return 0;
            }
        }

        public abstract bool Equals(Value value);
    }
}
