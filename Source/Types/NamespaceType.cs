namespace UglyLang.Source.Types
{
    /// <summary>
    /// A type representing a namespaced, used in imports
    /// </summary>
    public class NamespaceType : Type
    {
        public override bool Equals(Type other)
        {
            return other is NamespaceType;
        }

        public override bool DoesMatch(Type other, TypeParameterCollection coll)
        {
            return Equals(other);
        }

        public override string ToString()
        {
            return "NAMESPACE";
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
    }
}
