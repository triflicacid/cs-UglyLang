using UglyLang.Source.Types;

namespace UglyLang.Source.Values
{
    /// <summary>
    /// Value containing a type
    /// </summary>
    public class TypeValue : Value
    {
        public readonly Types.Type Value;

        public TypeValue(Types.Type type) : base(new TypeType())
        {
            Value = type;
        }

        public override bool IsTruthy()
        {
            return true;
        }

        public static TypeValue From(Value value)
        {
            throw new InvalidOperationException(value.Type.ToString());
        }

        public override Value? To(Types.Type type)
        {
            if (type is Any or TypeType)
                return new TypeValue(Value);
            if (type is StringType)
                return new StringValue(Value.ToString());
            return null;
        }

        public override bool Equals(Value value)
        {
            return value is TypeValue t && Value.Equals(t.Value);
        }
    }
}
