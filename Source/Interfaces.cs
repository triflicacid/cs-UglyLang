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

        public void CreateSymbol(string symbol, ISymbolValue value);

        public void SetSymbol(string symbol, ISymbolValue value);
    }
}
