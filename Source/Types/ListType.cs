using UglyLang.Source.Functions;
using UglyLang.Source.Functions.List;
using UglyLang.Source.Functions.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Types
{
    /// <summary>
    /// A collection of one type
    /// </summary>
    public class ListType : Type
    {
        public static readonly Dictionary<string, Variable> Properties = Variable.CreateDictionary(new Variable[]
        {
            new("Add", new FAdd()),
            new("Contains", new FContains()),
            new("Get", new FGet()),
            new("IndexOf", new FIndexOf()),
            new("Join", new FJoin()),
            new("Length", new FLength()),
            new("Remove", new FRemove()),
            new("RemoveAt", new FRemoveAt()),
            new("Reverse", new FReverse()),
            new("Set", new FSet()),
            new("Slice", new FSlice()),
        });

        private static readonly Function Constructor = new FListConstructor();

        public readonly Type Member;

        public ListType(Type member)
        {
            Member = member;
        }

        public override bool Equals(Type other)
        {
            return other is ListType list && Member.Equals(list.Member);
        }

        public override bool DoesMatch(Type other, TypeParameterCollection coll)
        {
            return other is TypeParameter || (other is ListType list && Member.DoesMatch(list.Member));
        }

        public override string ToString()
        {
            return Member.ToString() + "[]";
        }

        public override bool IsParameterised()
        {
            return Member.IsParameterised();
        }

        public override List<TypeParameter> GetTypeParameters()
        {
            return Member.GetTypeParameters();
        }

        public override TypeParameterCollection MatchParametersAgainst(Type t)
        {
            if (t is ListType list)
                return Member.MatchParametersAgainst(list.Member);
            return new();
        }

        public override Type ResolveParametersAgainst(TypeParameterCollection col)
        {
            return List(Member.ResolveParametersAgainst(col));
        }

        public override bool CanConstruct()
        {
            return Member is not Any && Member is not EmptyType && Member.CanConstruct();
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
            return v.Type is ListType list && (Member is Any || Member.Equals(list.Member));
        }
    }
}
