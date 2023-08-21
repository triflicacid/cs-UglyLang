using UglyLang.Source.Functions.String;
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
            return AsString();
        }

        public override Value? ConstructNoArgs(Context context)
        {
            return IntValue.Default();
        }

        public override Value? ConstructWithArgs(Context context, List<Value> args)
        {
            if (args.Count == 1)
            {
                IntValue? value = (IntValue?)args[0].To(this);
                if (value == null)
                {
                    context.Error = new(0, 0, Error.Types.Cast, string.Format("cannot cast {0} to {1}", args[0].Type, this));
                    return null;
                }

                return new IntValue(value.Value);
            }
            else
            {
                context.Error = new(0, 0, Error.Types.Type, string.Format("type {0} requires 1 argument, got {1}", this, args.Count));
                return null;
            }
        }

        public static string AsString()
        {
            return "INT";
        }

        public override bool IsTypeOf(Value v)
        {
            return v.Type is IntType;
        }
    }

    public class FloatType : PrimitiveType
    {
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
            return AsString();
        }

        public override Value? ConstructNoArgs(Context context)
        {
            return FloatValue.Default();
        }

        public override Value? ConstructWithArgs(Context context, List<Value> args)
        {
            if (args.Count == 1)
            {
                FloatValue? value = (FloatValue?)args[0].To(this);
                if (value == null)
                {
                    context.Error = new(0, 0, Error.Types.Cast, string.Format("cannot cast {0} to {1}", args[0].Type, this));
                    return null;
                }

                return new FloatValue(value.Value);
            }
            else
            {
                context.Error = new(0, 0, Error.Types.Type, string.Format("type {0} requires 1 argument, got {1}", this, args.Count));
                return null;
            }
        }

        public static string AsString()
        {
            return "FLOAT";
        }

        public override bool IsTypeOf(Value v)
        {
            return v.Type is FloatType;
        }
    }

    public class StringType : PrimitiveType
    {
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
            return AsString();
        }

        public override Value? ConstructNoArgs(Context context)
        {
            return StringValue.Default();
        }

        public override Value? ConstructWithArgs(Context context, List<Value> args)
        {
            if (args.Count == 1)
            {
                StringValue? value = (StringValue?)args[0].To(this);
                if (value == null)
                {
                    context.Error = new(0, 0, Error.Types.Cast, string.Format("cannot cast {0} to {1}", args[0].Type, this));
                    return null;
                }

                return new StringValue(value.Value);
            }
            else
            {
                context.Error = new(0, 0, Error.Types.Type, string.Format("type {0} requires 1 argument, got {1}", this, args.Count));
                return null;
            }
        }

        public override Dictionary<string, Property> GetProperties()
        {
            return Properties;
        }

        public static string AsString()
        {
            return "STRING";
        }

        public override bool IsTypeOf(Value v)
        {
            return v.Type is StringType;
        }
    }
}
