namespace UglyLang.Source.Types
{
    /// <summary>
    /// A type which identifies a type
    /// </summary>
    public class TypeType : Type
    {
        public override bool DoesMatch(Type other, TypeParameterCollection coll)
        {
            return other is TypeParameter or TypeType;
        }

        public override bool Equals(Type other)
        {
            return other is TypeType;
        }

        public override List<TypeParameter> GetTypeParameters()
        {
            throw new InvalidOperationException();
        }

        public override bool IsParameterised()
        {
            return false;
        }

        public override TypeParameterCollection MatchParametersAgainst(Type t)
        {
            return new();
        }

        public override Type ResolveParametersAgainst(TypeParameterCollection col)
        {
            return this;
        }

        public override string ToString()
        {
            return "TYPE";
        }
    }
}
