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

        public virtual Dictionary<string, Property> GetProperties()
        {
            return new();
        }

        /// <summary>
        /// Determine whether this type has a static property.
        /// </summary>
        public virtual bool HasStaticProperty(string name)
        {
            return false;
        }

        /// <summary>
        /// Get a static property, or null if it cannot be retrieved.
        /// </summary>
        public virtual Property? GetStaticProperty(string name)
        {
            return null;
        }

        /// <summary>
        /// Should this type allow its properties to change type? By default, this is true.
        /// </summary>
        public virtual bool HasRigidPropertyTypes()
        {
            return true;
        }

        /// <summary>
        /// Return whether the given type is type of this type.
        /// </summary>
        public abstract bool IsTypeOf(Value v);

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
        public static Type Map(Type t)
        {
            return new MapType(t);
        }
    }
}
