using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Values;

namespace UglyLang.Source.Types
{
    public class Property
    {
        private readonly string _Name;
        private ISymbolValue _Value;
        public bool IsReadonly = false;

        public Property(string name, ISymbolValue value, bool isReadonly = false)
        {
            _Name = name;
            _Value = value;
            IsReadonly = isReadonly;
        }

        public string GetName()
        {
            return _Name;
        }

        public ISymbolValue GetValue()
        {
            return _Value;
        }

        /// <summary>
        /// Set the value of said property. Return boolean success. Note, that we do not check if the types are compatibe; this is to be done elsewhere.
        /// </summary>
        public bool SetValue(ISymbolValue value)
        {
            if (IsReadonly)
            {
                return false;
            }

            _Value = value;
            return true;
        }

        public static Dictionary<string, Property> CreateDictionary(Property[] properties)
        {
            Dictionary<string, Property> dict = new();
            foreach (Property property in properties)
            {
                dict.Add(property.GetName(), property);
            }

            return dict;
        }
    }
}
