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

    public abstract class AbstractStackContext : ISymbolContainer
    {
        public readonly int LineNumber;
        public readonly int ColNumber;
        public readonly StackContextType Type;
        public readonly string Name;
        public Value? FunctionReturnValue = null;

        public AbstractStackContext(int line, int col, StackContextType type, string name)
        {
            LineNumber = line;
            ColNumber = col;
            Type = type;
            Name = name;
        }

        public abstract bool HasSymbol(string symbol);

        public abstract ISymbolValue GetSymbol(string symbol);

        public abstract void CreateSymbol(string symbol, ISymbolValue value);

        public abstract void SetSymbol(string symbol, ISymbolValue value);

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
            return string.Format("{0}, entered at line {1}, column {2}:", GetDetailString(), LineNumber + 1, ColNumber + 1);
        }
    }

    public class StackContext : AbstractStackContext
    {
        private readonly Dictionary<string, ISymbolValue> Symbols;
        private readonly TypeParameterCollection TypeParams;

        public StackContext(int line, int col, StackContextType type, string name, TypeParameterCollection? tParams = null)
        : base(line, col, type, name)
        {
            Symbols = new();
            TypeParams = tParams ?? new();
        }

        public override bool HasSymbol(string symbol)
        {
            return Symbols.ContainsKey(symbol);
        }

        public override ISymbolValue GetSymbol(string symbol)
        {
            if (!HasSymbol(symbol))
                throw new Exception(string.Format("Failed to get variable: name '{0}' could not be found", symbol));
            return Symbols[symbol];
        }

        public override void CreateSymbol(string symbol, ISymbolValue value)
        {
            Symbols.Add(symbol, value);
        }

        public override void SetSymbol(string symbol, ISymbolValue value)
        {
            Symbols[symbol] = value;
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

        public MethodStackContext(int line, int col, string name, UserValue owner)
        : base(line, col, StackContextType.Function, name, null)
        {
            Owner = owner;
            OwnerType = (UserType)owner.Type;
        }

        public override bool HasSymbol(string symbol)
        {
            return base.HasSymbol(symbol) || OwnerType.HasField(symbol) || OwnerType.HasMethod(symbol);
        }

        public override ISymbolValue GetSymbol(string symbol)
        {
            if (OwnerType.HasField(symbol))
                return Owner.FieldValues[symbol];
            if (OwnerType.HasMethod(symbol))
                return OwnerType.GetMethod(symbol);
            return base.GetSymbol(symbol);
        }

        public override void SetSymbol(string symbol, ISymbolValue value)
        {
            if (OwnerType.HasMethod(symbol))
                throw new InvalidOperationException();
            else if (OwnerType.HasField(symbol))
            {
                Owner.FieldValues[symbol] = (Value)value;
            }
            else
            {
                base.SetSymbol(symbol, value);
            }
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

        public ProxyStackContext(int line, int col, StackContextType type, string name, AbstractStackContext master)
        : base(line, col, type, name)
        {
            Master = master;
        }

        public override bool HasSymbol(string symbol)
        {
            return Master.HasSymbol(symbol);
        }

        public override ISymbolValue GetSymbol(string symbol)
        {
            return Master.GetSymbol(symbol);
        }

        public override void CreateSymbol(string symbol, ISymbolValue value)
        {
            Master.CreateSymbol(symbol, value);
        }

        public override void SetSymbol(string symbol, ISymbolValue value)
        {
            Master.SetSymbol(symbol, value);
        }

        public override TypeParameterCollection GetTypeParameters()
        {
            return Master.GetTypeParameters();
        }
    }
}
