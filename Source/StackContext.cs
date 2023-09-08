using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source
{
    public enum StackContextType
    {
        File,
        Function,
        DoBlock,
    }

    public abstract class AbstractStackContext : ILocatable, IVariableContainer
    {
        public readonly int LineNumber;
        public readonly int ColNumber;
        public readonly StackContextType Type;
        public ILocatable? Initiator;
        public readonly string Name;
        public Value? FunctionReturnValue = null;

        public AbstractStackContext(int line, int col, StackContextType type, ILocatable? initiator, string name)
        {
            LineNumber = line;
            ColNumber = col;
            Type = type;
            Initiator = initiator;
            Name = name;
        }

        public abstract bool HasSymbol(string symbol);

        public abstract Variable GetSymbol(string symbol);

        public abstract void CreateSymbol(Variable value);

        public abstract TypeParameterCollection GetTypeParameters();

        /// <summary>
        /// Used to supply the first half of the string, before the entry location
        /// </summary>
        public virtual string GetDetailString()
        {
            string s = Type switch
            {
                StackContextType.File => "File",
                StackContextType.Function => "Function",
                StackContextType.DoBlock => "Do",
                _ => throw new NotSupportedException(Type.ToString()),
            };
            return Name.Length == 0 ? s : s + " " + Name;
        }

        public sealed override string ToString()
        {
            return string.Format("{0}, entered at line {1}, column {2}", GetDetailString(), LineNumber + 1, ColNumber + 1);
        }

        public string GetInitiatedString()
        {
            if (Initiator == null)
                return "";

            return Type switch
            {
                StackContextType.Function => string.Format("Function {0} defined at {1}:{2}", Name, Initiator.GetLineNumber() + 1, Initiator.GetColumnNumber() + 1),
                _ => "",
            };
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

    public class StackContext : AbstractStackContext
    {
        private readonly Dictionary<string, Variable> Symbols;
        private readonly TypeParameterCollection TypeParams;

        public StackContext(int line, int col, StackContextType type, ILocatable? initiator, string name, TypeParameterCollection? tParams = null)
        : base(line, col, type, initiator, name)
        {
            Symbols = new();
            TypeParams = tParams ?? new();
        }

        public override bool HasSymbol(string symbol)
        {
            return Symbols.ContainsKey(symbol);
        }

        public override Variable GetSymbol(string symbol)
        {
            if (!HasSymbol(symbol))
                throw new Exception(string.Format("Failed to get variable: name '{0}' could not be found", symbol));
            return Symbols[symbol];
        }

        public override void CreateSymbol(Variable value)
        {
            Symbols.Add(value.GetName(), value);
        }

        public override TypeParameterCollection GetTypeParameters()
        {
            return TypeParams;
        }

        public NamespaceValue ExportToNamespace()
        {
            NamespaceValue ns = new();

            foreach (var pair in Symbols)
            {
                ns.Value.Add(pair.Key, pair.Value);
            }

            return ns;
        }
    }

    /// <summary>
    /// Extends from StackContext, used when calling methods. Acts as a proxy for properties/methods on  this.Owner.
    /// </summary>
    public class MethodStackContext : StackContext
    {
        public readonly UserValue Owner;
        public readonly UserType OwnerType;

        public MethodStackContext(int line, int col, ILocatable? initiator, string name, UserValue owner)
        : base(line, col, StackContextType.Function, initiator, name, null)
        {
            Owner = owner;
            OwnerType = (UserType)owner.Type;
        }

        public override bool HasSymbol(string symbol)
        {
            return base.HasSymbol(symbol) || OwnerType.HasField(symbol) || OwnerType.HasMethod(symbol);
        }

        public override Variable GetSymbol(string symbol)
        {
            if (OwnerType.HasField(symbol))
                return Owner.FieldValues[symbol];
            if (OwnerType.HasMethod(symbol))
                return OwnerType.GetMethod(symbol);
            return base.GetSymbol(symbol);
        }

        public override string GetDetailString()
        {
            return "Method " + OwnerType.Name + ":" + Name;
        }
    }

    /// <summary>
    /// A stack context, but forwards allocations to another context
    /// </summary>
    public class ProxyStackContext : AbstractStackContext
    {
        public readonly AbstractStackContext Master;

        public ProxyStackContext(int line, int col, StackContextType type, ILocatable? initiator, string name, AbstractStackContext master)
        : base(line, col, type, initiator, name)
        {
            Master = master;
        }

        public override bool HasSymbol(string symbol)
        {
            return Master.HasSymbol(symbol);
        }

        public override Variable GetSymbol(string symbol)
        {
            return Master.GetSymbol(symbol);
        }

        public override void CreateSymbol(Variable value)
        {
            Master.CreateSymbol(value);
        }

        public override TypeParameterCollection GetTypeParameters()
        {
            return Master.GetTypeParameters();
        }
    }
}
