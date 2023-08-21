using UglyLang.Source.Values;

namespace UglyLang.Source.Types
{
    /// <summary>
    /// A loose type wrapper representing an absence
    /// </summary>
    public class EmptyType : Type
    {
        public override bool Equals(Type other)
        {
            return other is EmptyType;
        }

        public override bool DoesMatch(Type other, TypeParameterCollection coll)
        {
            return other is EmptyType;
        }

        public override string ToString()
        {
            return "EMPTY";
        }

        public override bool IsParameterised()
        {
            return false;
        }

        public override List<TypeParameter> GetTypeParameters()
        {
            throw new InvalidOperationException();
        }

        public override TypeParameterCollection MatchParametersAgainst(Type t)
        {
            return new();
        }

        public override Type ResolveParametersAgainst(TypeParameterCollection col)
        {
            return this;
        }

        public override bool IsTypeOf(Value v)
        {
            return v.Type is EmptyType;
        }
    }
}
