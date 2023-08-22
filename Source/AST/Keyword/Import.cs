using UglyLang.Source.Values;
using static UglyLang.Source.ParseOptions;

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
        /// Load the contents of the given filename in this.Content. Return isOk.
        /// </summary>
        public bool Load(ParseOptions options)
        {
            Parser p = new(options);
            p.ParseFile(Filename, LineNumber, ColumnNumber);

            if (p.IsError())
            {
                return false;
            }

            Content = p.GetAST();

            return true;
        }

        public override Signal Action(Context context)
        {
            if (Content == null)
                throw new InvalidOperationException();

            ImportCache cache = context.ParseOptions.FileSources[Filename];
            NamespaceValue ns;
            Signal s;

            // Is the namespace cached? If not, evaluate it and cache the result.
            if (cache.Namespace == null)
            {
                // Push the new stack context
                if (Namespace == null)
                {
                    context.PushProxyStackContext(LineNumber, ColumnNumber, StackContextType.File, Filename);
                }
                else
                {
                    context.PushStackContext(LineNumber, ColumnNumber, StackContextType.File, Filename);
                }

                s = Content.Evaluate(context);
                if (s == Signal.ERROR)
                    return s;

                var oldContext = context.PopStackContext();

                ns = ((StackContext)oldContext).ExportToNamespace();
                cache.Namespace = ns;
            }
            else
            {
                ns = cache.Namespace;
                s = Signal.NONE;
            }

            // Bind namespace to a symbol
            if (Namespace != null)
            {
                // Export into a namespace and create new variable
                if (context.HasSymbol(Namespace.Symbol))
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Name, string.Format("{0} is already defined", Namespace.Symbol));
                    return Signal.ERROR;
                }
                else
                {
                    context.CreateSymbol(Namespace.Symbol, ns);
                }
            }

            return s == Signal.EXIT_PROG ? s : Signal.NONE;
        }
    }
}
