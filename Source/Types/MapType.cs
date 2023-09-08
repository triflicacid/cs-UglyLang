using UglyLang.Source.Functions;
using UglyLang.Source.Functions.Map;
using UglyLang.Source.Functions.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Types
{
    /// <summary>
    /// A type which maps strings to a type
    /// </summary>
    public class MapType : Type
    {
        public static readonly Dictionary<string, Variable> Properties = Variable.CreateDictionary(new Variable[]
        {
            new("Delete", new FDelete()),
            new("Get", new FGet()),
            new("Has", new FHas()),
            new("Keys", new FKeys()),
            new("Set", new FSet()),
            new("Size", new FSize()),
        });

        private static readonly Function Constructor = new FMapConstructor();

        public readonly Type ValueType;

        public MapType(Type valueType)
        {
            ValueType = valueType;
        }

        public override bool Equals(Type other)
        {
            return other is MapType m && ValueType.Equals(m.ValueType);
        }

        public override bool DoesMatch(Type other, TypeParameterCollection coll)
        {
            return other is TypeParameter || (other is MapType map && ValueType.DoesMatch(map.ValueType));
        }

        public override string ToString()
        {
            return "MAP[" + ValueType.ToString() + "]";
        }

        public override bool IsParameterised()
        {
            return ValueType.IsParameterised();
        }

        public override List<TypeParameter> GetTypeParameters()
        {
            return ValueType.GetTypeParameters();
        }

        public override TypeParameterCollection MatchParametersAgainst(Type t)
        {
            if (t is MapType map)
                return ValueType.MatchParametersAgainst(map.ValueType);
            return new();
        }

        public override Type ResolveParametersAgainst(TypeParameterCollection col)
        {
            return Map(ValueType.ResolveParametersAgainst(col));
        }

        public override bool CanConstruct()
        {
            return ValueType is not Any && ValueType is not EmptyType && ValueType.CanConstruct();
        }

        public override Function GetConstructorFunction()
        {
            return Constructor;
        }

        public override Dictionary<string, Variable> GetProperties()
        {
            return Properties;
        }

        public override bool IsTypeOf(Value v)
        {
            return v.Type is MapType map && (ValueType is Any || ValueType.Equals(map.ValueType));
        }
    }
}
