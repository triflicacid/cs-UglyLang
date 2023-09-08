using UglyLang.Source.Functions;
using UglyLang.Source.Types;
using static UglyLang.Source.Functions.Function;

namespace UglyLang.Source.Values
{
    public class UserValue : Value
    {
        /// Contains all the values of associated fields in UserType. Must be set up externally.
        public Dictionary<string, Variable> FieldValues = new();

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
            if (type is Any)
                return this;
            if (type is UserType ut && ((UserType)Type).Id == ut.Id)
                return this;
            if (type is StringType)
                return new StringValue(((UserType)Type).Name);
            return null;
        }

        protected override bool HasPropertyExtra(string name)
        {
            return ((UserType)Type).HasField(name) || ((UserType)Type).HasMethod(name) || ((UserType)Type).HasStaticField(name);
        }

        protected override Variable? GetPropertyExtra(string name)
        {
            if (((UserType)Type).HasMethod(name))
            {
                Variable method = ((UserType)Type).GetMethod(name);
                Variable v = new(method, new Method((Function)method.GetValue(), this))
                {
                    IsReadonly = true
                };
                return v;
            }
            else if (FieldValues.ContainsKey(name))
            {
                return FieldValues[name];
            }
            else if (((UserType)Type).HasStaticField(name))
            {
                return ((UserType)Type).GetStaticField(name);
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
                FieldValues[name].SetValue((Value)value);
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
