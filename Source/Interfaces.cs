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
    /// Represent a structure whichcontains symbols
    /// </summary>
    public interface ISymbolContainer
    {
        public bool HasSymbol(string symbol);

        public ISymbolValue GetSymbol(string symbol);

        public virtual bool CanCreateSymbol(string name)
        {
            return true;
        }

        public void CreateSymbol(string symbol, ISymbolValue value);

        public void SetSymbol(string symbol, ISymbolValue value);
    }

    public interface ICallable
    {
        /// <summary>
        /// Call the given function with said arguments. Redirect call to CallOverload once the correct overload has been found. All stack context preparations should be done before calling this method.
        /// </summary>
        public Signal Call(Context context, List<Value> arguments, int lineNumber, int colNumber);
    }
}
