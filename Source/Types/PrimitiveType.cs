using UglyLang.Source.Functions;
using UglyLang.Source.Functions.String;
using UglyLang.Source.Functions.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Types
{
    public abstract class PrimitiveType : Type
    {
        public override bool CanConstruct()
        {
            return true;
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

    public class IntType : PrimitiveType
    {
        private static readonly Function Constructor = new FIntConstructor();

        public override bool Equals(Type other)
        {
            return other is IntType;
        }

        public override bool DoesMatch(Type other, TypeParameterCollection coll)
        {
            return other is TypeParameter or IntType or FloatType;
        }

        public override string ToString()
        {
            return "INT";
        }

        public override Function GetConstructorFunction()
        {
            return Constructor;
        }

        public override bool IsTypeOf(Value v)
        {
            return v.Type is IntType;
        }
    }

    public class FloatType : PrimitiveType
    {
        private static readonly Function Constructor = new FFloatConstructor();

        public override bool Equals(Type other)
        {
            return other is FloatType;
        }

        public override bool DoesMatch(Type other, TypeParameterCollection coll)
        {
            return other is TypeParameter or FloatType or IntType;
        }

        public override string ToString()
        {
            return "FLOAT";
        }

        public override bool IsTypeOf(Value v)
        {
            return v.Type is FloatType;
        }

        public override Function GetConstructorFunction()
        {
            return Constructor;
        }
    }

    public class StringType : PrimitiveType
    {
        private static readonly Function Constructor = new FStringConstructor();

        public static readonly Dictionary<string, Property> Properties = Property.CreateDictionary(new Property[]
        {
            new("Contains", new FContains()),
            new("IndexOf", new FIndexOf()),
            new("Length", new FLength()),
            new("Lower", new FLower()),
            new("Reverse", new FReverse()),
            new("Split", new FSplit()),
            new("Slice", new FSlice()),
            new("Title", new FTitle()),
            new("Upper", new FUpper()),
        });

        public override bool Equals(Type other)
        {
            return other is StringType;
        }

        public override bool DoesMatch(Type other, TypeParameterCollection coll)
        {
            return other is TypeParameter or StringType;
        }

        public override string ToString()
        {
            return "STRING";
        }

        public override Dictionary<string, Property> GetProperties()
        {
            return Properties;
        }

        public override bool IsTypeOf(Value v)
        {
            return v.Type is StringType;
        }

        public override Function GetConstructorFunction()
        {
            return Constructor;
        }
    }
}
