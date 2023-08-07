using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UglyLang.Source.Types
{
    public abstract class Type
    {
        public abstract bool Equals(Type other);

        public abstract bool DoesMatch(Type other);

        public override string ToString()
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Construct type from a string. Return NULL if there is a failure.
        /// </summary>
        public static Type? FromString(string s)
        {
            if (s == IntType.AsString()) return new IntType();
            if (s == FloatType.AsString()) return new FloatType();
            if (s == StringType.AsString()) return new StringType();
            if (s.Length > 2 && s[^1] == ']' && s[^2] == '[')
            {
                Type? member = FromString(s[..^2]);
                return member == null ? null : new ListType(member); 
            }
            return null;
        }
    }
}
