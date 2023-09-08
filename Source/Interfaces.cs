using UglyLang.Source.Values;

namespace UglyLang.Source
{
    /// <summary>
    /// If inherited from, this symbol will be defined as a global symbol on any new Context.
    /// </summary>
    public interface IDefinedGlobally : ISymbolValue
    {
        public string GetDefinedName();
    }

    /// <summary>
    /// Identifies an object as potentially being assigned to a symbol
    /// </summary>
    public interface ISymbolValue
    { }

    /// <summary>
    /// Represent a structure which contains variables
    /// </summary>
    public interface IVariableContainer
    {
        public bool HasSymbol(string symbol);

        public Variable GetSymbol(string symbol);

        public virtual bool CanCreateSymbol(string name)
        {
            return true;
        }

        public void CreateSymbol(Variable value);
    }

    public interface ICallable
    {
        /// <summary>
        /// Call the given function with said arguments. Redirect call to CallOverload once the correct overload has been found. All stack context preparations should be done before calling this method.
        /// </summary>
        public Signal Call(Context context, List<Value> arguments, int lineNumber, int colNumber);
    }

    public interface ILocatable
    {
        public int GetLineNumber();
        public int GetColumnNumber();

        public Location GetLocation()
        {
            return new(GetLineNumber(), GetColumnNumber());
        }
    }

    public class Location : ILocatable
    {
        private readonly int LineNumber;
        private readonly int ColumnNumber;

        public Location(int lineNumber, int columnNumber)
        {
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
        }

        public int GetLineNumber()
        {
            return LineNumber;
        }

        public int GetColumnNumber()
        {
            return ColumnNumber;
        }
    }
}
