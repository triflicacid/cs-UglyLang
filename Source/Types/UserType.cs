using UglyLang.Source.Values;

namespace UglyLang.Source.Types
{
    public class UserType : Type, ISymbolValue
    {
        public readonly string Name;

        public UserType(string name)
        {
            Name = name;
        }

        public override bool DoesMatch(Type other, TypeParameterCollection coll)
        {
            return other is TypeParameter || Equals(other); // User types should only have once instance
        }

        public override bool Equals(Type other)
        {
            return other is UserType t && t == this; // User types should only have once instance
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
            throw new();
        }

        public override Type ResolveParametersAgainst(TypeParameterCollection col)
        {
            return this;
        }

        public override bool IsTypeOf(Value v)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
