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
        /// Same as MatchParametersAgainst, but resolves any type parameters in the collection.
        /// </summary>
        public abstract Type ResolveParametersAgainst(TypeParameterCollection col);

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
        /// Return properties attached to this type. By default, this is empty.
        /// </summary>
        public virtual Dictionary<string, Property> GetProperties()
        {
            return new();
        }

        public static readonly Type AnyT = new Any();
        public static readonly Type EmptyT = new EmptyType();
        public static readonly Type IntT = new IntType();
        public static readonly Type FloatT = new FloatType();
        public static readonly Type StringT = new StringType();
        public static readonly Type TypeT = new TypeType();
        public static Type List(Type t)
        {
            return new ListType(t);
        }
    }

    public class Property
    {
        private string _Name;
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
