using UglyLang.Source.Functions;
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
        /// Can this type be constructed? This is checked before GetConstructorFunction() is called.
        /// </summary>
        public virtual bool CanConstruct()
        {
            return false;
        }

        /// <summary>
        /// Get the defined constructr function for the given type. Note that this is different from the other Construct functions.
        /// </summary>
        public virtual Function GetConstructorFunction()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Attempt to construct this type by applying the given argument values to GetConstructorFunction().
        /// </summary>
        public (Signal, Value?) Construct(Context context, List<Value> arguments, int lineNumber, int colNumber)
        {
            if (!CanConstruct())
            {
                context.Error = new(lineNumber, colNumber, Error.Types.Type, string.Format("type {0} cannot be constructed", this));
                return (Signal.ERROR, null);
            }

            Function constructor = GetConstructorFunction();
            Value? finalValue = null;
            string name = ToString();

            if (this is UserType userType)
            {
                UserValue initial = userType.ConstructInitialValue();
                finalValue = initial;
                context.PushMethodStackContext(lineNumber, colNumber, userType, name, initial);
            }
            else
            {
                context.PushStackContext(lineNumber, colNumber, StackContextType.Function, null, name);
            }

            // Built-in types requires the type itself to be passed in for reference
            if (this is not UserType)
                arguments.Insert(0, new TypeValue(this));

            // Call function with given arguments
            Signal signal = constructor.Call(context, arguments, lineNumber, colNumber + name.Length);
            if (signal == Signal.ERROR)
                return (signal, null);

            Value returnValue = context.GetFunctionReturnValue() ?? new EmptyValue();
            if (this is UserType)
            {
                if (returnValue.Type is not EmptyType)
                {
                    context.Error = new(lineNumber, colNumber, Error.Types.Type, string.Format("{0} constructor: expected constructor to return EMPTY, got {1}", name, returnValue.Type));
                    return (Signal.ERROR, null);
                }
            }
            else
            {
                if (returnValue.Type.Equals(this))
                {
                    finalValue = returnValue;
                }
                else
                {
                    context.Error = new(lineNumber, colNumber, Error.Types.Type, string.Format("{0} constructor: expected constructor to return {1}, got {2}", name, this, returnValue.Type));
                    return (Signal.ERROR, null);
                }
            }

            // Pop stack context
            context.PopStackContext();

            return (Signal.NONE, finalValue);
        }

        public virtual Dictionary<string, Variable> GetProperties()
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
        public virtual Variable? GetStaticProperty(string name)
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
        public static ListType List(Type t)
        {
            return new ListType(t);
        }
        public static MapType Map(Type t)
        {
            return new MapType(t);
        }
    }
}
