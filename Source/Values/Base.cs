using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
