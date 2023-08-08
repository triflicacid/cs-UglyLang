using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Functions;
using UglyLang.Source.Types;

namespace UglyLang.Source.Values
{
    /// <summary>
    /// Identifies an object as potentially being assigned to a symbol
    /// </summary>
    public interface ISymbolValue
    {

    }

    public abstract class Value : ISymbolValue
    {
        public Types.Type Type;

        /// <summary>
        /// Cast this value to the given type, or return null.
        /// </summary>
        public abstract Value? To(Types.Type type);

        /// <summary>
        /// Return whether or not this instance of the current type can be considered as truthy
        /// </summary>
        public abstract bool IsTruthy();

        public bool HasProperty(string name)
        {
            return Type.Properties.ContainsKey(name) || HasPropertyExtra(name);
        }

        protected virtual bool HasPropertyExtra(string name)
        {
            return false;
        }

        public ISymbolValue GetProperty(string name)
        {
            if (!HasProperty(name)) throw new InvalidOperationException(name);

            ISymbolValue value = Type.Properties.ContainsKey(name) ? Type.Properties[name] : (ISymbolValue) GetPropertyExtra(name);
            if (value is Function func)
            {
                value = new FunctionContext(func, this);
            }

            return value;
        }

        protected virtual ISymbolValue? GetPropertyExtra(string name)
        {
            return null;
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
