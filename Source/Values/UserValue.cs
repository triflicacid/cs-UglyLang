using UglyLang.Source.Types;
using static UglyLang.Source.Functions.Function;

namespace UglyLang.Source.Values
{
    public class UserValue : Value
    {
        /// Contains all the values of associated fields in UserType. Must be set up externally.
        public Dictionary<string, Value> FieldValues = new();

        public UserValue(UserType type) : base(type)
        { }

        public override bool Equals(Value value)
        {
            return value is UserValue uv && uv == this;
        }

        public override bool IsTruthy()
        {
            return true;
        }

        public override Value? To(Types.Type type)
        {
            if (type is Any) return this;
            if (type is UserType ut && ((UserType)Type).Id == ut.Id) return this;
            if (type is StringType) return new StringValue("<TYPE " + ((UserType)Type).Name + ">");
            return null;
        }

        protected override bool HasPropertyExtra(string name)
        {
            return ((UserType)Type).HasField(name) || ((UserType)Type).HasMethod(name);
        }

        protected override Property? GetPropertyExtra(string name)
        {
            if (((UserType)Type).HasMethod(name))
            {
                return new(name, new Method(((UserType)Type).GetMethod(name), this), true);
            }
            else if (FieldValues.ContainsKey(name))
            {
                return new(name, FieldValues[name]);
            }
            else
            {
                return base.GetPropertyExtra(name);
            }
        }

        protected override bool SetPropertyExtra(string name, ISymbolValue value)
        {
            if (FieldValues.ContainsKey(name))
            {
                FieldValues[name] = (Value)value;
                return true;
            }
            else
            {
                return base.SetPropertyExtra(name, value);
            }
        }

        public override bool AreFunctionsContextual()
        {
            return false;
        }
    }
}
