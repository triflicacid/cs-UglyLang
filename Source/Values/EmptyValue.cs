using UglyLang.Source.Types;

namespace UglyLang.Source.Values
{
    /// <summary>
    /// A value which contains nothing - undefined/null
    /// </summary>
    public class EmptyValue : Value
    {
        public EmptyValue() : base(new EmptyType())
        { }

        public EmptyValue(Types.Type type) : base(type)
        { }

        public static EmptyValue From(Value value)
        {
            throw new NotSupportedException();
        }

        public override bool IsTruthy()
        {
            return false;
        }

        public override Value? To(Types.Type type)
        {
            if (Type is EmptyType)
            {
                if (type is Any or EmptyType)
                    return this;
                if (type is IntType)
                    return IntValue.Default();
                if (type is FloatType)
                    return FloatValue.Default();
                if (type is StringType)
                    return StringValue.Default();
                return null;

            }
            else
            {
                if (type is StringType)
                    return new StringValue("EMPTY<" + Type + ">");
                if (type is Any || type.Equals(Type))
                    return this;
                return null;
            }
        }

        public override bool Equals(Value value)
        {
            return value is EmptyValue eValue && eValue.Type.Equals(Type);
        }
    }
}
