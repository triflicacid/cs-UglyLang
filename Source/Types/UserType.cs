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
        public NamespaceValue Statics = new(); // Store static members
        public readonly Function? Constructor;

        public UserType(string name)
        {
            Name = name;
            Fields = new();
            Methods = new();
            Constructor = null;
            Id = GlobalId++;
        }

        public UserType(UserTypeDataContainer data)
        {
            Name = data.Name;
            Fields = data.Fields;
            Methods = data.Functions;
            Constructor = data.Constructor.CountOverloads() == 0 ? null : data.Constructor;
            Id = GlobalId++;
        }

        /// <summary>
        /// Construct an "empty" version of this type, this is what is passed to the constructor
        /// </summary>
        public UserValue ConstructInitialValue()
        {
            UserValue value = new(this);
            foreach (var t in GetAllFields())
            {
                value.FieldValues.Add(t.Item1, new EmptyValue(t.Item2));
            }

            return value;
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

        public Tuple<string, Type>[] GetAllFields()
        {
            return Fields.Select(p => new Tuple<string, Type>(p.Key, p.Value)).ToArray();
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
            return Constructor is not null;
        }

        public override Function GetConstructorFunction()
        {
            return Constructor ?? throw new NullReferenceException();
        }

        public override bool HasStaticProperty(string name)
        {
            return Statics.HasSymbol(name);
        }

        public override Property? GetStaticProperty(string name)
        {
            return new(name, Statics.GetSymbol(name), true);
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
        public readonly Function Constructor = new();

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
            if (Functions.ContainsKey(symbol))
                return Functions[symbol];
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
