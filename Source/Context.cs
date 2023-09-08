using UglyLang.Source.Types;
using UglyLang.Source.Values;
using static UglyLang.Source.ParseOptions;

namespace UglyLang.Source
{
    public class Context : IVariableContainer
    {
        private readonly List<IVariableContainer> Stack; // Stack of pushed contexts. It can be any symbol container, but at runtime it is always an AbstractStackContext.
        public Error? Error = null;
        public readonly ParseOptions ParseOptions;

        /// <summary>
        /// Create a new execution context. Provide the ParseOptions instance used to parse the file, then provide the filename of the entry file.
        /// </summary>
        public Context(ParseOptions options, string filename)
        {
            Stack = new() { new StackContext(0, 0, StackContextType.File, null, filename) };
            ParseOptions = options;
            CreateSymbol(new("_BaseDir", new StringValue(options.BaseDirectory))
            {
                IsReadonly = true
            });
        }

        /// <summary>
        /// Does the given variable exist?
        /// </summary>
        public bool HasSymbol(string name)
        {
            foreach (var d in Stack)
            {
                if (d.HasSymbol(name))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Get the value of the given variable or throw an error. Looks from the topmost scope downwards.
        /// </summary>
        public Variable GetSymbol(string name)
        {
            for (int i = Stack.Count - 1; i >= 0; i--)
            {
                if (Stack[i].HasSymbol(name))
                    return Stack[i].GetSymbol(name);
            }

            throw new ArgumentException(name);
        }

        /// <summary>
        /// Can the followig symbol be created?
        /// </summary>
        public bool CanCreateSymbol(string name)
        {
            return !Stack[^1].HasSymbol(name);
        }

        /// <summary>
        /// Creates a new symbol in the topmost scope and sets it
        /// </summary>
        public void CreateSymbol(Variable value)
        {
            if (!CanCreateSymbol(value.GetName()))
                throw new InvalidOperationException(value.GetName());

            Stack[^1].CreateSymbol(value);
        }

        private string GetSourceLine(string filename, int lineNumber)
        {
            try
            {
                return ParseOptions.FileSources[filename].GetSourceLines()[lineNumber];
            }
            catch
            {
                return $"({filename} line {lineNumber + 1})";
            }
        }

        private string GetSourceLine(int stackIndex, int lineNumber)
        {
            string? filename = null;
            for (int i = stackIndex; i >= 0; i--)
            {
                if (Stack[i] is AbstractStackContext frame)
                {
                    if (frame.Type == StackContextType.File)
                    {
                        filename = frame.Name;
                        break;
                    }
                }

            }

            if (filename == null)
                return "(unknown)";

            return GetSourceLine(filename, lineNumber);
        }

        public string GetErrorString()
        {
            return Error == null ? "" : ErrorToString(Error);
        }

        private string GetLineError(int stackIdx, int lineNumber, int colNumber, string pad1 = "", string? pad2 = null)
        {
            string line = GetSourceLine(stackIdx, lineNumber);
            int origLength = line.Length;
            line = line.TrimStart();
            string lineNumberS = (lineNumber + 1).ToString();
            int colIdx = colNumber - (origLength - line.Length);

            string str = Environment.NewLine + pad1 + (lineNumber + 1) + " | " + line;
            string pre = new(' ', lineNumberS.Length);
            if (colIdx >= line.Length)
            {
                // Console.WriteLine($"BEFORE: #{lineNumber} '{line}' ({line.Length}) {colNumber} {origLength} {colIdx}'");
                throw new ArgumentOutOfRangeException("colIdx=" + colIdx);
            }
            else
            {
                string before = Parser.NonWhitespaceRegex.Replace(line[..colIdx], " ");
                string after = Parser.NonWhitespaceRegex.Replace(line[colIdx..], " ");
                str += Environment.NewLine + (pad2 ?? pad1) + pre + "   " + before + "^" + after + Environment.NewLine;
            }

            return str;
        }

        /// <summary>
        /// Given an error, return the error as a string using context information
        /// </summary>
        private string ErrorToString(Error error)
        {
            string str = "";
            for (int i = 0; i < Stack.Count; i++)
            {
                if (Stack[i] is AbstractStackContext frame)
                {
                    // Error information
                    str += frame.ToString() + Environment.NewLine;

                    if (i != 0)
                    {
                        str += GetLineError(i == 0 ? 0 : i - 1, frame.LineNumber, frame.ColNumber);

                        if (frame.Initiator != null)
                        {
                            string initStr = frame.GetInitiatedString();
                            if (initStr.Length > 0)
                                str += " > " + initStr + Environment.NewLine + "   " + GetLineError(i == 0 ? 0 : i - 1, frame.Initiator.GetLineNumber(), frame.Initiator.GetColumnNumber(), "   ", null);
                        }
                    }
                }

            }

            str += error.ToString();
            str += Environment.NewLine + GetLineError(Stack.Count - 1, error.LineNumber, error.ColumnNumber);

            if (error.AppendString.Length > 0)
                str += error.AppendString;
            if (error.AdditionalSource != null)
                str += (error.AppendString.Length == 0 ? "" : Environment.NewLine) + GetLineError(Stack.Count - 1, error.AdditionalSource.GetLineNumber(), error.AdditionalSource.GetColumnNumber());

            return str;
        }

        /// <summary>
        /// Push a new item on to the stack
        /// </summary>
        public void PushStack(IVariableContainer container)
        {
            Stack.Add(container);
        }

        /// <summary>
        /// Push a new stack context
        /// </summary>
        public void PushStackContext(int line, int col, StackContextType type, ILocatable? initiator, string name, TypeParameterCollection? typeParams = null)
        {
            Stack.Add(new StackContext(line, col, type, initiator, name, typeParams));
        }

        /// <summary>
        /// Push a new method stack context
        /// </summary>
        public void PushMethodStackContext(int line, int col, ILocatable? initiator, string name, UserValue owner)
        {
            Stack.Add(new MethodStackContext(line, col, initiator, name, owner));
        }

        /// <summary>
        /// Push a new proxy stack context forthe latest stack context
        /// </summary>
        public void PushProxyStackContext(int line, int col, StackContextType type, ILocatable? initiator, string name)
        {
            Stack.Add(new ProxyStackContext(line, col, type, initiator, name, (AbstractStackContext)Stack[^1]));
        }

        public AbstractStackContext PeekStackContext()
        {
            return (AbstractStackContext)Stack[^1];
        }

        public IVariableContainer PeekStack()
        {
            return Stack[^1];
        }

        /// <summary>
        /// Pop the latest stack context
        /// </summary>
        public AbstractStackContext PopStackContext()
        {
            return (AbstractStackContext)PopStack();
        }

        /// <summary>
        /// Pop the latest item from the stack
        /// </summary>
        public IVariableContainer PopStack()
        {
            if (Stack.Count < 2)
                throw new InvalidOperationException();
            IVariableContainer peek = Stack[^1];
            Stack.RemoveAt(Stack.Count - 1);
            return peek;
        }

        public void SetFunctionReturnValue(Value? value)
        {
            var context = (AbstractStackContext)Stack[^1];
            context.FunctionReturnValue = value;
        }

        public Value? GetFunctionReturnValue()
        {
            return ((AbstractStackContext)Stack[^1]).FunctionReturnValue;
        }

        /// <summary>
        /// Merge the given collection with the latest collection on the stack
        /// </summary>
        public void MergeTypeParams(TypeParameterCollection c)
        {
            ((AbstractStackContext)Stack[^1]).GetTypeParameters().MergeWith(c);
        }

        /// <summary>
        /// From the current scope down, get any bound type parameters
        /// </summary>
        public TypeParameterCollection GetBoundTypeParams()
        {
            TypeParameterCollection c = new();
            foreach (IVariableContainer context in Stack)
            {
                if (context is AbstractStackContext stackContext)
                    c.MergeWith(stackContext.GetTypeParameters());
            }
            return c;
        }

        /// <summary>
        /// Import a new file, read the contents and execute it. If `source` is provided, do not read the file and use this parameter instead. The boolean indicates whether the cached Namespace for the import should be replaced.
        /// </summary>
        public (Signal, NamespaceValue?) Import(string path, int entryLine = 0, int entryColumn = 0, bool replaceCachedNamespace = false, string? source = null)
        {
            // Parse the source
            Parser p = new(ParseOptions);
            if (source == null)
            {
                p.ParseFile(path, entryLine, entryColumn);
            }
            else
            {
                p.ParseSource(path, source, entryLine, entryColumn);
            }

            // Was there an error during parsing?
            if (p.IsError())
            {
                Error = new(entryLine, entryColumn, Error.Types.Import, string.Format("whilst importing '{0}'", path))
                {
                    AppendString = p.GetErrorString()
                };
                return (Signal.ERROR, null);
            }

            ImportCache cache = ParseOptions.FileSources[path];
            NamespaceValue ns;
            Signal s;

            // Is the namespace cached? If not, evaluate it and cache the result.
            if (replaceCachedNamespace || cache.Namespace == null)
            {
                // Push the new stack context
                PushStackContext(entryLine, entryColumn, StackContextType.File, null, path);

                s = p.GetAST().Evaluate(this);
                if (s == Signal.ERROR)
                    return (s, null);

                ns = ((StackContext)PopStackContext()).ExportToNamespace();
                cache.Namespace = ns;
            }
            else
            {
                ns = cache.Namespace;
                s = Signal.NONE;
            }

            return (s == Signal.EXIT_PROG ? s : Signal.NONE, ns);
        }

        public void InitialiseGlobals()
        {
            // Add all globally defined functions
            var type = typeof(IDefinedGlobally);
            foreach (IDefinedGlobally x in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface)
                .Select(o => (IDefinedGlobally)Activator.CreateInstance(o)))
            {
                CreateSymbol(new(x.GetDefinedName(), x));
            }
        }
    }
}
