using UglyLang.Source.Functions;
using UglyLang.Source.Values;

namespace UglyLang.Source.Types
{
    public class UserType : Type, ISymbolValue
    {
        private static int GlobalId = 0; // Give each type instance a new ID

        public readonly int Id;
        public readonly string Name;
        private readonly Dictionary<string, Type> Fields; // Contains the fields of an instance
        private readonly Dictionary<string, Function> Methods; // Contains methods

        public UserType(string name)
        {
            Name = name;
            Fields = new();
            Methods = new();
            Id = GlobalId++;
        }

        public UserType(UserTypeDataContainer data)
        {
            Name = data.Name;
            Fields = data.Fields;
            Methods = data.Functions;
            Id = GlobalId++;
        }

        public bool HasMethod(string methodName)
        {
            return Methods.ContainsKey(methodName);
        }

        public Function GetMethod(string methodName)
        {
            return Methods[methodName];
        }

        public string[] GetAllMethods()
        {
            return Methods.Select(p => p.Key).ToArray();
        }

        public bool HasField(string fieldName)
        {
            return Fields.ContainsKey(fieldName);
        }

        public Type GetField(string fieldName)
        {
            return Fields[fieldName];
        }

        public string[] GetAllFields()
        {
            return Fields.Select(p => p.Key).ToArray();
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool DoesMatch(Type other, TypeParameterCollection coll)
        {
            return other is TypeParameter || Equals(other); // User types should only have once instance
        }

        public override bool Equals(Type other)
        {
            return other is UserType t && t == this; // User types should only have once instance
        }

        public override List<TypeParameter> GetTypeParameters()
        {
            throw new InvalidOperationException();
        }

        public override bool IsParameterised()
        {
            return false;
        }

        public override TypeParameterCollection MatchParametersAgainst(Type t)
        {
            throw new();
        }

        public override Type ResolveParametersAgainst(TypeParameterCollection col)
        {
            return this;
        }

        public override bool CanConstruct()
        {
            return true;
        }

        private bool ConstructFieldOnValue(Context context, UserValue value, string fieldName)
        {
            Type fieldType = GetField(fieldName);
            if (fieldType.IsParameterised())
            {
                Type resolved = fieldType.ResolveParametersAgainst(context.GetBoundTypeParams());
                if (resolved.IsParameterised())
                {
                    context.Error = new(0, 0, Error.Types.Type, string.Format("cannot resolve parameterised type '{0}' (partially resolved to {1}; in construction of type {2}, field {3})", fieldType, resolved, this, fieldName));
                    return false;
                }
            }

            if (fieldType.CanConstruct())
            {
                Value? fieldValue = fieldType.ConstructNoArgs(context);
                if (fieldValue == null)
                {
                    context.Error = new(0, 0, Error.Types.Type, string.Format("cannot construct type {0} with no arguments (in construction of type {1}, field {2})", fieldType, this, fieldName));
                    return false;
                }
                else
                {
                    value.FieldValues.Add(fieldName, fieldValue);
                    return true;
                }
            }
            else
            {
                context.Error = new(0, 0, Error.Types.Type, string.Format("cannot construct type {0} (in construction of type {1}, field {2})", fieldType, this, fieldName));
                return false;
            }
        }

        public override Value? ConstructNoArgs(Context context)
        {
            UserValue value = new(this);
            foreach (string fieldName in GetAllFields())
            {
                if (!ConstructFieldOnValue(context, value, fieldName)) return null;
            }

            return value;
        }

        public override Value? ConstructWithArgs(Context context, List<Value> args)
        {
            string[] fieldNames = GetAllFields();
            if (fieldNames.Length < args.Count)
            {
                context.Error = new(0, 0, Error.Types.Type, string.Format("type {0} expects at most {1} arguments, got {2}", this, fieldNames.Length, args.Count));
                return null;
            }

            UserValue value = new(this);

            // Assign given values
            for (int i = 0; i < args.Count; i++)
            {
                Type fieldType = GetField(fieldNames[i]);
                if (args[i].Type is Any || args[i].Type.DoesMatch(fieldType))
                {
                    Value fieldValue;
                    if (args[i].Type is not Any && !args[i].Equals(fieldType))
                    {
                        Value? casted = args[i].To(fieldType);
                        if (casted == null)
                        {
                            context.Error = new(0, 0, Error.Types.Cast, string.Format("cannot cast {0} to {1} (in construction of type {2}, field {3})", args[i].Type, fieldType, this, fieldNames[i]));
                            return null;
                        }

                        fieldValue = casted;
                    }
                    else
                    {
                        fieldValue = args[i];
                    }

                    value.FieldValues.Add(fieldNames[i], fieldValue);
                }
                else
                {
                    context.Error = new(0, 0, Error.Types.Type, string.Format("expected {0}, got {1} (in construction of type {2}, field {3})", fieldType, args[i].Type, this, fieldNames[i]));
                    return null;
                }
            }

            // Construct remaining values
            for (int i = args.Count; i < fieldNames.Length; i++)
            {
                if (!ConstructFieldOnValue(context, value, fieldNames[i])) return null;
            }

            return value;
        }

        public override bool IsTypeOf(Value v)
        {
            return v.Type is UserType ut && Id == ut.Id;
        }
    }

    /// <summary>
    /// A special symbol container used by TypeKeywordNode.
    /// </summary>
    public class UserTypeDataContainer : ISymbolContainer
    {
        public readonly string Name;
        public readonly Dictionary<string, Function> Functions = new();
        public readonly Dictionary<string, Type> Fields = new();

        public UserTypeDataContainer(string name)
        {
            Name = name;
        }

        public void CreateSymbol(string symbol, ISymbolValue value)
        {
            if (value is Function f)
            {
                Functions.Add(symbol, f);
            }
            else
            {
                throw new ArgumentException(value.ToString());
            }
        }

        public ISymbolValue GetSymbol(string symbol)
        {
            if (Functions.ContainsKey(symbol)) return Functions[symbol];
            throw new ArgumentException(symbol);
        }

        public bool HasSymbol(string symbol)
        {
            return Functions.ContainsKey(symbol) || Fields.ContainsKey(symbol);
        }

        public void SetSymbol(string symbol, ISymbolValue value)
        {
            if (value is Function f)
            {
                Functions[symbol] = f;
            }
            else
            {
                throw new ArgumentException(value.ToString());
            }
        }
    }
}
