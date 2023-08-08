using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Values;

namespace UglyLang.Source.Types
{
    public abstract class Type
    {
        public Dictionary<string, ISymbolValue> Properties = new();

        public abstract bool Equals(Type other);

        public bool DoesMatch(Type other)
        {
            return DoesMatch(other, new TypeParameterCollection());
        }

        public abstract bool DoesMatch(Type other, TypeParameterCollection coll);

        public override string ToString()
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Return whether or not this type is parameterised
        /// </summary>
        public abstract bool IsParameterised();

        /// <summary>
        /// Extract all type parameters contained in this type
        /// </summary>
        public abstract List<TypeParameter> GetTypeParameters();

        /// <summary>
        /// Match type parameters in this against the given type. E.g., if this is "a[]" and the parameter is "INT[]" then a=INT
        /// </summary>
        public abstract TypeParameterCollection MatchParametersAgainst(Type t);

        /// <summary>
        /// Can this type be constructed?
        /// </summary>
        public virtual bool CanConstruct()
        {
            return false;
        }

        /// <summary>
        /// Attempt to construct this type with no arguments, or return null
        /// </summary>
        public virtual Value? ConstructNoArgs(Context context)
        {
            context.Error = new(0, 0, Error.Types.Type, string.Format("cannot construct type {0} with no arguments", this));
            return null;
        }

        /// <summary>
        /// Attempt to construct this type with the given arguments, or return null
        /// </summary>
        public virtual Value? ConstructWithArgs(Context context, List<Value> args)
        {
            context.Error = new(0, 0, Error.Types.Type, string.Format("type {0} accepts no arguments, got {1}", this, args.Count));
            return null;
        }

        /// <summary>
        /// Construct type from a string. Return NULL if there is a failure.
        /// </summary>
        public static Type? FromString(string s, bool allowParams = false)
        {
            if (s == IntType.AsString()) return new IntType();
            if (s == FloatType.AsString()) return new FloatType();
            if (s == StringType.AsString()) return new StringType();
            if (s.Length > 2 && s[^1] == ']' && s[^2] == '[')
            {
                Type? member = FromString(s[..^2], allowParams);
                return member == null ? null : new ListType(member); 
            }
            if (allowParams && Parser.IsValidSymbol(s)) return new TypeParameter(s);
            return null;
        }
    }
}
