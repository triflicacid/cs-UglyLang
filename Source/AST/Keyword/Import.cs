namespace UglyLang.Source.AST.Keyword
{
    /// <summary>
    /// Keyword to import another resource
    /// </summary>
    public class ImportKeywordNode : KeywordNode
    {
        public readonly string Filename;
        public ASTStructure? Content = null;
        public readonly SymbolNode? Namespace;

        public ImportKeywordNode(string filename, SymbolNode? ns)
        {
            Filename = filename;
            Namespace = ns;
        }

        /// <summary>
        /// Load the contents of the given filename in this.Content. Return (isOk, errorString).
        /// </summary>
        public (bool, string) Load(string baseDirectory, Dictionary<string, string> sources)
        {
            if (Content != null) throw new InvalidOperationException();

            string fullpath = Path.Join(baseDirectory, Filename);

            // Does the source already exist?
            if (sources.ContainsKey(Filename))
            {
                return (false, new Error(LineNumber, ColumnNumber, Error.Types.Import, string.Format("'{0}' has already been imported", fullpath)).ToString());
            }

            // Attempt to locate the source
            if (File.Exists(fullpath))
            {
                // Read the file
                string source = File.ReadAllText(fullpath);

                // Attempt to parse the source
                Parser p = new(baseDirectory);
                p.Parse(Filename, source, sources);

                // Assign the parsed AST
                Content = p.AST;

                if (p.Error == null)
                {
                    return (true, "");
                }
                else
                {
                    return (false, p.GetErrorString());
                }
            }
            else
            {
                return (false, new Error(LineNumber, ColumnNumber, Error.Types.Import, string.Format("'{0}' cannot be found", fullpath)).ToString());
            }
        }

        public override Signal Action(Context context, ISymbolContainer container)
        {
            if (Content == null) throw new InvalidOperationException();

            // Push the new stack context
            if (Namespace == null)
            {
                context.PushProxyStackContext(LineNumber, ColumnNumber, StackContextType.File, Filename);
            }
            else
            {
                context.PushStackContext(LineNumber, ColumnNumber, StackContextType.File, Filename);
            }

            Signal s = Content.Evaluate(context, container);
            if (s == Signal.ERROR) return s;

            var oldContext = context.PopStackContext();

            if (Namespace != null)
            {
                // Export into a namespace and create new variable
                if (container.HasSymbol(Namespace.Symbol))
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Name, string.Format("{0} is already defined", Namespace.Symbol));
                    return Signal.ERROR;
                }
                else
                {
                    container.CreateSymbol(Namespace.Symbol, ((StackContext)oldContext).ExportToNamespace());
                }
            }

            return s == Signal.EXIT_PROG ? s : Signal.NONE;
        }
    }
}
