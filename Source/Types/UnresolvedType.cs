namespace UglyLang.Source.Types
{
    /// <summary>
    /// Wraps a string which will be resolved to a type
    /// </summary>
    public class UnresolvedType
    {
        public readonly string Value;

        public UnresolvedType(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Resolve said type, or return NULL if error.
        /// </summary>
        public virtual Type? Resolve(Context context)
        {
            return Resolve(context, Value);
        }

        public static Type? Resolve(Context context, string value)
        {
            // Built-in?
            if (value == IntType.AsString()) return new IntType();
            if (value == FloatType.AsString()) return new FloatType();
            if (value == StringType.AsString()) return new StringType();
            if (value.Length > 2 && value[^1] == ']' && value[^2] == '[')
            {
                Type? member = Resolve(context, value[..^2]);
                return member == null ? null : new ListType(member);
            }

            // User type?
            if (context.HasVariable(value) && context.GetVariable(value) is UserType t)
            {
                return t;
            }

            // Assume it is a type parameter
            return new TypeParameter(value);
        }
    }

    /// <summary>
    /// A wrapper for an unresolved type which resolves to a pre-defined type
    /// </summary>
    public class ResolvedType : UnresolvedType
    {
        public readonly Type ResolveTo;

        public ResolvedType(Type type) : base(type.ToString())
        {
            ResolveTo = type;
        }

        public override Type? Resolve(Context context)
        {
            return ResolveTo;
        }

        // Constants and functions to generate ResolvedType instances
        public static readonly ResolvedType Empty = new(new EmptyType());
        public static readonly ResolvedType Any = new(new Any());
        public static readonly ResolvedType Int = new(new IntType());
        public static readonly ResolvedType Float = new(new FloatType());
        public static readonly ResolvedType String = new(new StringType());
        public static readonly ResolvedType Type = new(new TypeType());

        public static ResolvedType List(Type member)
        {
            return new ResolvedType(new ListType(member));
        }

        public static ResolvedType Param(string s)
        {
            return new ResolvedType(new TypeParameter(s));
        }
    }
}
