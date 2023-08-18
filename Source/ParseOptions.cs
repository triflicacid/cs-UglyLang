using UglyLang.Source.AST;
using UglyLang.Source.Values;

namespace UglyLang.Source
{
    public class ParseOptions
    {
        /// <summary>
        /// Before a keyword: what should we expect to parse?
        /// </summary>
        public enum Before
        {
            NONE,
            SYMBOL, // SymbolNode
            CHAINED_SYMBOL, // SymbolNode or ChainedSymbolNode
        }

        /// <summary>
        /// After a keyword: what should we expect to parse?
        /// </summary>
        public enum After
        {
            NONE,
            EXPR, // ExprNode
            TYPE, // SymbolNode with .Symbol set to the type string
            STRING, // StringNode
        }

        public class ImportCache
        {
            public readonly string Source; // String source of the file
            public ASTStructure? AST; // Parsed AST structure of the file. NULL if the file hasn't been parsed yet.
            public NamespaceValue? Namespace; // Namespace resulting from evaluation. NULL if the file import hasn't been evaluated yet.
            private string[]? Lines;

            public ImportCache(string source)
            {
                Source = source;
            }

            public string[] GetSourceLines()
            {
                Lines ??= Source.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                return Lines;
            }
        }

        public readonly string BaseDirectory;
        public readonly Dictionary<string, ImportCache> FileSources = new();
        private readonly List<string> ImportedFiles = new(); // Current chain of imported files to avoid cyclic imports
        private readonly List<(string, Error)> Errors = new(); // Error list, (filepath, error)

        public ParseOptions(string baseDirectory)
        {
            BaseDirectory = baseDirectory;
        }

        /// <summary>
        /// Returns the current directory we are in (in terms of imports).
        /// </summary>
        /// <returns></returns>
        public string GetCurrentDirectory()
        {
            return ImportedFiles.Count == 0 ? BaseDirectory : Path.Join(BaseDirectory, Path.GetDirectoryName(ImportedFiles[^1]));
        }

        /// <summary>
        /// Add a new error for the latest imported file
        /// </summary>
        public void AddError(Error err)
        {
            Errors.Add((ImportedFiles[^1], err));
        }

        /// <summary>
        /// Add a new error for the given file
        /// </summary>
        public void AddError(string filename, Error err)
        {
            Errors.Add((filename, err));
        }

        /// <summary>
        /// Insert a new error for the given file at the front of the error list
        /// </summary>
        public void InsertError(string filename, Error err)
        {
            Errors.Insert(0, (filename, err));
        }

        public void AddImport(string filename)
        {
            ImportedFiles.Add(filename);
        }

        public string PeekImport()
        {
            return ImportedFiles[^1];
        }

        public void PopImport()
        {
            ImportedFiles.RemoveAt(ImportedFiles.Count - 1);
        }

        public bool HasImportedFile(string filename)
        {
            return ImportedFiles.Contains(filename);
        }

        /// <summary>
        /// Return if there is an error
        /// </summary>
        /// <returns></returns>
        public bool IsError()
        {
            return Errors.Count > 0;
        }

        public string GetErrorString()
        {
            if (Errors.Count == 0)
                return "";

            string str = "";
            foreach ((string filename, Error err) in Errors)
            {
                str += "File " + filename + ": " + err.ToString() + Environment.NewLine;

                if (FileSources.TryGetValue(filename, out ImportCache? cache))
                {
                    string line = cache.GetSourceLines()[err.LineNumber];
                    int origLength = line.Length;
                    line = line.TrimStart();
                    string lineNumberS = (err.LineNumber + 1).ToString();
                    int colIdx = err.ColumnNumber - (origLength - line.Length);

                    str += Environment.NewLine + (err.LineNumber + 1) + " | " + line;
                    string pre = new(' ', lineNumberS.Length);
                    string before = Parser.NonWhitespaceRegex.Replace(line[..colIdx], " ");
                    string after = Parser.NonWhitespaceRegex.Replace(line[colIdx..], " ");
                    str += Environment.NewLine + pre + "   " + before + "^" + after + Environment.NewLine;
                }
            }

            return str;
        }
    }
}
