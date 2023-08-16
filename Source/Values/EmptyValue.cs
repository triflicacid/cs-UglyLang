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
            if (type is Any or EmptyType) return this;
            if (type is IntType) return IntValue.Default();
            if (type is FloatType) return FloatValue.Default();
            if (type is StringType) return StringValue.Default();
            return null;
        }

        public override bool Equals(Value value)
        {
            return value is EmptyValue;
        }
    }
}
