using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source
{
    public enum StackContextType
    {
        File,
        Function
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

        public sealed override string ToString()
        {
            string inside = "unknown";
            switch (Type)
            {
                case StackContextType.File:
                    inside = "File";
                    break;
                case StackContextType.Function:
                    inside = "Function";
                    break;
            }

            return string.Format("{0} {1}, entered at line {2}, column {3}:", inside, Name, LineNumber + 1, ColNumber + 1);
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
