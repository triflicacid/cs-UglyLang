using UglyLang.Source.Functions.List;
using UglyLang.Source.Values;

namespace UglyLang.Source.Types
{
    /// <summary>
    /// A collection of one type
    /// </summary>
    public class ListType : Type
    {
        public static readonly Dictionary<string, Property> Properties = Property.CreateDictionary(new Property[]
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
            if (t is ListType list) return Member.MatchParametersAgainst(list.Member);
            return new();
        }

        public override Type ResolveParametersAgainst(TypeParameterCollection col)
        {
            return new ListType(Member.ResolveParametersAgainst(col));
        }

        public override bool CanConstruct()
        {
            return true;
        }

        public override Value? ConstructNoArgs(Context context)
        {
            return new ListValue(Member);
        }

        public override Value? ConstructWithArgs(Context context, List<Value> args)
        {
            ListValue list = new(Member);

            foreach (Value arg in args)
            {
                Value? value = arg.To(Member);
                if (value == null)
                {
                    context.Error = new(0, 0, Error.Types.Cast, string.Format("cannot cast {0} to {1}", arg.Type, Member));
                    return null;
                }

                list.Value.Add(value);
            }

            return list;
        }

        public override Dictionary<string, Property> GetProperties()
        {
            return Properties;
        }
    }
}
