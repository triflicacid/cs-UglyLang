using UglyLang.Source.AST;
using UglyLang.Source.Values;

namespace UglyLang.Source.Types
{
    /// <summary>
    /// Wraps a string which will be resolved to a type
    /// </summary>
    public class UnresolvedType
    {
        public readonly AbstractSymbolNode Value;

        public UnresolvedType(string value)
        {
            Value = new SymbolNode(value);
        }

        public UnresolvedType(AbstractSymbolNode value)
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

        public static Type? Resolve(Context context, AbstractSymbolNode node)
        {
            if (node is SymbolNode symbolNode)
            {
                string value = symbolNode.Symbol;

                // Primitives?
                if (value == "INT")
                    return Type.IntT;
                if (value == "FLOAT")
                    return Type.FloatT;
                if (value == "STRING")
                    return Type.StringT;
                if (value == "TYPE")
                    return Type.TypeT;

                // Namespace? Hide it from public viewing.
                if (value == "NAMESPACE")
                    return null;

                // Generic list and map types?
                if (value == "MAP")
                    return Type.Map(Type.AnyT);

                if (value == "LIST")
                    return Type.List(Type.AnyT);

                // List of a type?
                if (value.Length > 2 && value[^1] == ']' && value[^2] == '[')
                {
                    Type? member = Resolve(context, new SymbolNode(value[..^2]));
                    return member == null ? null : Type.List(member);
                }

                // Map of a type
                if (value.StartsWith("MAP[") && value[^1] == ']')
                {
                    Type? member = Resolve(context, new SymbolNode(value[4..^1]));
                    return member == null ? null : Type.Map(member);
                }

                // User type?
                if (context.HasSymbol(value) && context.GetSymbol(value).GetValue() is UserType t)
                    return t;

                // Assume it is a type parameter
                return new TypeParameter(value);
            }
            else if (node is ChainedSymbolNode chain)
            {
                // Property access, so must be a UserType (or nothing)
                Value? value = chain.Evaluate(context);
                return value is TypeValue tval ? tval.Value : null;
            }
            else
            {
                throw new NotSupportedException();
            }
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
