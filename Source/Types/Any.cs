using UglyLang.Source.Values;

namespace UglyLang.Source.Types
{
    /// <summary>
    /// A loose type wrapper representing any type
    /// </summary>
    public class Any : Type
    {
        public override bool Equals(Type other)
        {
            return other is Any;
        }

        public override bool DoesMatch(Type other, TypeParameterCollection coll)
        {
            return true;
        }

        public override string ToString()
        {
            return "ANY";
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
            return true;
        }
    }
}
