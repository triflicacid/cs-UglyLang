using UglyLang.Source.Functions;
using UglyLang.Source.Values;

namespace UglyLang.Source.Types
{
    public class UserType : Type, ILocatable, ISymbolValue
    {
        private static int GlobalId = 0; // Give each type instance a new ID

        public readonly int Id;
        public readonly string Name;
        private readonly Dictionary<string, Variable> Fields; // Contains the fields of an instance
        private readonly Dictionary<string, Variable> Methods; // Contains methods
        public NamespaceValue Statics = new(); // Store static members
        public readonly Function? Constructor;
        public readonly int LineNumber;
        public readonly int ColNumber;

        public UserType(string name, int lineNo, int colNo)
        {
            Name = name;
            Fields = new();
            Methods = new();
            Constructor = null;
            Id = GlobalId++;
            ColNumber = colNo;
            LineNumber = lineNo;
        }

        public UserType(UserTypeDataContainer data)
        {
            Name = data.Name;
            Fields = data.Fields;
            Methods = data.Functions;
            Constructor = data.Constructor.CountOverloads() == 0 ? null : data.Constructor;
            Id = GlobalId++;
            ColNumber = data.ColNumber;
            LineNumber = data.LineNumber;
        }

        /// <summary>
        /// Construct an "empty" version of this type, this is what is passed to the constructor
        /// </summary>
        public UserValue ConstructInitialValue()
        {
            UserValue value = new(this);
            foreach (var t in GetAllFields())
            {
                value.FieldValues.Add(t.Item1, new(t.Item2, new EmptyValue((Type)t.Item2.GetValue())));
            }

            return value;
        }

        public bool HasMethod(string methodName)
        {
            return Methods.ContainsKey(methodName);
        }

        public Variable GetMethod(string methodName)
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

        public Variable GetField(string fieldName)
        {
            return Fields[fieldName];
        }

        public Tuple<string, Variable>[] GetAllFields()
        {
            return Fields.Select(p => new Tuple<string, Variable>(p.Key, p.Value)).ToArray();
        }

        public bool HasStaticField(string fieldName)
        {
            return Statics.HasSymbol(fieldName);
        }

        public Variable GetStaticField(string fieldName)
        {
            return Statics.GetSymbol(fieldName);
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

        public override Variable? GetStaticProperty(string name)
        {
            return Statics.GetSymbol(name);
        }

        public override bool IsTypeOf(Value v)
        {
            return v.Type is UserType ut && Id == ut.Id;
        }

        public int GetLineNumber()
        {
            return LineNumber;
        }

        public int GetColumnNumber()
        {
            return ColNumber;
        }
    }

    /// <summary>
    /// A special symbol container used by TypeKeywordNode.
    /// </summary>
    public class UserTypeDataContainer : IVariableContainer
    {
        public readonly string Name;
        public readonly Dictionary<string, Variable> Functions = new();
        public readonly Dictionary<string, Variable> Fields = new();
        public readonly Function Constructor = new();
        public int LineNumber = -1;
        public int ColNumber = -1;

        public UserTypeDataContainer(string name)
        {
            Name = name;
        }

        public void CreateSymbol(Variable value)
        {
            if (value.GetValue() is Function)
            {
                Functions.Add(value.GetName(), value);
            }
            else
            {
                throw new ArgumentException(value.ToString());
            }
        }

        public Variable GetSymbol(string symbol)
        {
            if (Functions.ContainsKey(symbol))
                return Functions[symbol];
            throw new ArgumentException(symbol);
        }

        public bool HasSymbol(string symbol)
        {
            return Functions.ContainsKey(symbol) || Fields.ContainsKey(symbol);
        }
    }
}
