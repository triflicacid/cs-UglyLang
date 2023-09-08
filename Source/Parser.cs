using System.Text.RegularExpressions;
using UglyLang.Source.AST;
using UglyLang.Source.AST.Keyword;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source
{
    public partial class Parser
    {
        private static readonly char CommentChar = ';';
        private static readonly char BlockCommentChar = ':';
        private static readonly char TypeLiteralChar = '@';

        private ASTStructure? AST = null;
        private readonly ParseOptions Options;

        public Parser(ParseOptions options)
        {
            Options = options;
        }

        private void AddError(Error err)
        {
            Options.AddError(err);
        }

        /// <summary>
        /// Parse the given file using this.Options. If OK, set this.AST.
        /// </summary>
        public void ParseFile(string filename, int lineNo, int colNo)
        {
            string fullpath = Path.Join(Options.GetCurrentDirectory(), filename);

            // Does the source already exist?
            if (Options.HasImportedFile(filename))
            {
                AddError(new(lineNo, colNo, Error.Types.Import, string.Format("'{0}' has already been imported", fullpath)));
                return;
            }

            ParseOptions.ImportCache? cache;

            // Already parsed?
            if (Options.FileSources.TryGetValue(filename, out cache) && cache.AST != null)
            {
                AST = cache.AST;
            }

            // Attempt to locate the source
            else if (cache != null || File.Exists(fullpath))
            {
                Options.AddImport(filename);

                // Read the file/fetch the source
                string source;
                if (cache != null)
                {
                    source = cache.Source;
                }
                else
                {
                    source = File.ReadAllText(fullpath);
                    cache = new(source);
                    Options.FileSources.Add(filename, cache);
                }

                // Attempt to parse the source
                Parse(filename, source);

                if (!IsError())
                {
                    Options.PopImport();
                    cache.AST ??= AST;
                }

                return;
            }
            else
            {
                Options.AddError(filename, new(lineNo, colNo, Error.Types.Import, string.Format("'{0}' cannot be found", fullpath)));
            }
        }

        /// <summary>
        /// Parse the given file using this.Options. If OK, set this.AST.
        /// </summary>
        public void ParseSource(string filename, string source, int entryLine = 0, int entryCol = 0)
        {
            // Does the source already exist?
            if (Options.HasImportedFile(filename))
            {
                AddError(new(entryLine, entryCol, Error.Types.Import, string.Format("'{0}' has already been imported", Path.Join(Options.BaseDirectory, filename))));
                return;
            }

            ParseOptions.ImportCache? cache = null;

            // Set source
            if (Options.FileSources.TryGetValue(filename, out cache))
            {
                if (cache.AST != null && source == cache.Source)
                {
                    AST = cache.AST;
                    return;
                }
            }
            else
            {
                cache = new(source);
                Options.FileSources.Add(filename, cache);
            }

            Options.AddImport(filename);

            // Attempt to parse the source
            Parse(filename, source);
            cache.AST = AST;

            if (!IsError())
            {
                Options.PopImport();
            }
        }

        /// <summary>
        /// Parse the given file using this.Options. If OK, set this.AST.
        /// </summary>
        private void Parse(string filename, string source)
        {
            AST = null;

            // Nested structure
            Stack<ASTStructure> trees = new();
            trees.Push(new());
            Stack<(KeywordNode, KeywordInfo)> infoStack = new(); // Contains information on what keyword triggered a new ASTStructure to be pushed to `trees`. COntains a reference to the keyword ASTNode.
            bool inComment = false;

            string[] lines = source.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            for (int lineNumber = 0, colNumber = 0; lineNumber < lines.Length; lineNumber++, colNumber = 0)
            {
                string line = lines[lineNumber];

                // Eat whitespace
                while (colNumber < line.Length && char.IsWhiteSpace(line[colNumber]))
                    colNumber++;

                int startOfLineCol = colNumber;

                // Is the line empty?
                if (colNumber == line.Length)
                {
                    continue;
                }

                // Is a comment?
                if (line[colNumber] == CommentChar)
                {
                    if (colNumber + 1 < line.Length && line[colNumber + 1] == BlockCommentChar)
                    {
                        inComment = true;
                    }
                    continue;
                }
                else if (inComment && line[colNumber] == BlockCommentChar && colNumber + 1 < line.Length && line[colNumber + 1] == CommentChar)
                {
                    inComment = false;
                    continue;
                }
                else if (inComment)
                {
                    continue;
                }

                // Extract keyword
                string keyword = "";
                int keywordCol = colNumber;

                while (colNumber < line.Length && char.IsLetter(line[colNumber]))
                {
                    keyword += line[colNumber++];
                }

                // Check if the keyword exists
                if (keyword.Length == 0)
                {
                    AddError(new(lineNumber, colNumber, Error.Types.Syntax, string.Format("expected keyword, got '{0}'", line[colNumber])));
                    break;
                }
                else if (!KeywordInfo.Lookup.ContainsKey(keyword)) // Is it a valid keyword?
                {
                    AddError(new(lineNumber, colNumber - keyword.Length, Error.Types.Syntax, string.Format("unknown keyword {0}", keyword)));
                    break;
                }

                // Fetch keyword information - this will tell us what to parse.
                var keywordInfo = KeywordInfo.Lookup[keyword];

                // Check if the keyword is allowed here
                if (infoStack.Count > 0)
                {
                    (KeywordNode _, KeywordInfo kwInfo) = infoStack.Peek();

                    if (kwInfo.AllowUnder != null && !kwInfo.AllowUnder.Contains(keyword))
                    {
                        AddError(new(lineNumber, colNumber - keyword.Length, Error.Types.Syntax, string.Format("keyword {0} is not permitted in a {1} context", keyword, kwInfo.Keyword)));
                        break;
                    }

                    else if (keywordInfo.AllowAbove != null && !keywordInfo.AllowAbove.Contains(kwInfo.Keyword))
                    {
                        AddError(new(lineNumber, colNumber - keyword.Length, Error.Types.Syntax, string.Format("keyword {0} is not permitted in a {1} context", keyword, kwInfo.Keyword)));
                        break;
                    }
                }
                else if (keywordInfo.AllowAbove != null)
                {
                    AddError(new(lineNumber, colNumber - keyword.Length, Error.Types.Syntax, string.Format("keyword {0} is not permitted outside of a context", keyword)));
                    break;
                }

                // Is define keyword? This one needs special parsing.
                if (keyword == "DEF")
                {
                    // Eat whitespace
                    while (colNumber < line.Length && char.IsWhiteSpace(line[colNumber]))
                        colNumber++;

                    // Extract name
                    int beforeColNumber = colNumber;
                    while (colNumber < line.Length && char.IsLetterOrDigit(line[colNumber]))
                        colNumber++;
                    string functionName = line[beforeColNumber..colNumber];

                    if (!IsValidSymbol(functionName))
                    {
                        AddError(new(lineNumber, beforeColNumber, Error.Types.Syntax, string.Format("invalid symbol \"{0}\"", functionName)));
                        break;
                    }

                    // Eat whitespace
                    while (colNumber < line.Length && char.IsWhiteSpace(line[colNumber]))
                        colNumber++;

                    // Should be a colon or end of line
                    if (colNumber == line.Length)
                    {
                        // Create keyword node and add to tree structure
                        DefKeywordNode defNode = new(functionName, new(), ResolvedType.Empty)
                        {
                            LineNumber = lineNumber,
                            ColumnNumber = startOfLineCol
                        };
                        trees.Peek().AddNode(defNode);
                        trees.Push(new());
                        infoStack.Push((defNode, keywordInfo));
                        continue;
                    }
                    else if (line[colNumber] != ':')
                    {
                        string got = colNumber == line.Length ? "end of line" : line[colNumber].ToString();
                        AddError(new(lineNumber, colNumber, Error.Types.Syntax, string.Format("expected ':', got {0}", got)));
                        break;
                    }
                    colNumber++;

                    // Eat whitespace
                    while (colNumber < line.Length && char.IsWhiteSpace(line[colNumber]))
                        colNumber++;

                    beforeColNumber = colNumber;

                    // Return type
                    UnresolvedType returnType;
                    if (colNumber < line.Length && line[colNumber] == '<')
                    {
                        returnType = ResolvedType.Empty; // No return type
                    }
                    else
                    {
                        while (colNumber < line.Length && !char.IsWhiteSpace(line[colNumber]))
                            colNumber++;

                        string returnTypeString = line[beforeColNumber..colNumber];
                        returnType = new UnresolvedType(returnTypeString);
                    }

                    // Eat whitespace
                    while (colNumber < line.Length && char.IsWhiteSpace(line[colNumber]))
                        colNumber++;

                    // Variables to store function arguments and type constraints
                    List<(string, UnresolvedType)> argumentPairs;
                    Dictionary<string, List<UnresolvedType>>? constraints = null;

                    // Expect angled bracket
                    beforeColNumber = colNumber;
                    if (colNumber == line.Length)
                    {
                        // No arguments
                        argumentPairs = new();
                    }
                    else if (line[colNumber] != '<')
                    {
                        string got = colNumber == line.Length ? "end of line" : line[colNumber].ToString();
                        AddError(new(lineNumber, colNumber, Error.Types.Syntax, string.Format("expected '<', got {0}", got)));
                        break;
                    }
                    else
                    {
                        colNumber++;

                        // Eat whitespace
                        while (colNumber < line.Length && char.IsWhiteSpace(line[colNumber]))
                            colNumber++;

                        if (colNumber == line.Length)
                        {
                            AddError(new(lineNumber, colNumber, Error.Types.Syntax, "expected '>' or symbol, got end of line"));
                            break;
                        }

                        if (line[colNumber] == '>')
                        {
                            colNumber++;
                            argumentPairs = new();
                        }
                        else
                        {
                            (argumentPairs, int endCol) = ParseArgumentDeclaration(line[beforeColNumber..], beforeColNumber, lineNumber);

                            colNumber = beforeColNumber + endCol;

                            if (IsError())
                                break;
                        }

                        // Eat whitespace
                        while (colNumber < line.Length && char.IsWhiteSpace(line[colNumber]))
                            colNumber++;

                        // "WHERE" contraint clause?
                        if (line[colNumber..].StartsWith("WHERE"))
                        {
                            // Eat non-whitespace to skip keyword
                            while (colNumber < line.Length && !char.IsWhiteSpace(line[colNumber]))
                                colNumber++;

                            (constraints, int endCol) = ParseTypeConstraint(line[colNumber..], colNumber, lineNumber);
                            colNumber += endCol;

                            if (IsError())
                                break;
                        }
                    }

                    // Create keyword node and add to tree structure
                    DefKeywordNode node = new(functionName, argumentPairs, returnType, constraints)
                    {
                        LineNumber = lineNumber,
                        ColumnNumber = startOfLineCol
                    };
                    trees.Peek().AddNode(node);

                    // Should be at the end of the line
                    if (colNumber < line.Length)
                    {
                        AddError(new(lineNumber, colNumber, Error.Types.Syntax, string.Format("expected end of line, got '{0}'", line[colNumber])));
                        break;
                    }

                    // Nest and add a new tree
                    trees.Push(new());
                    infoStack.Push((node, keywordInfo));

                    continue;
                }

                else if (keyword == "NEW")
                {
                    // Eat whitespace
                    while (colNumber < line.Length && char.IsWhiteSpace(line[colNumber]))
                        colNumber++;

                    List<(string, UnresolvedType)> argumentPairs;

                    // Expect angled bracket
                    int beforeColNumber = colNumber;
                    if (colNumber == line.Length)
                    {
                        // No arguments
                        argumentPairs = new();
                    }
                    else if (line[colNumber] != '<')
                    {
                        string got = colNumber == line.Length ? "end of line" : line[colNumber].ToString();
                        AddError(new(lineNumber, colNumber, Error.Types.Syntax, string.Format("expected '<', got {0}", got)));
                        break;
                    }
                    else
                    {
                        colNumber++;

                        // Eat whitespace
                        while (colNumber < line.Length && char.IsWhiteSpace(line[colNumber]))
                            colNumber++;

                        if (colNumber == line.Length)
                        {
                            AddError(new(lineNumber, colNumber, Error.Types.Syntax, "expected '>' or symbol, got end of line"));
                            break;
                        }

                        if (line[colNumber] == '>')
                        {
                            colNumber++;
                            argumentPairs = new();
                        }
                        else
                        {
                            (argumentPairs, int endCol) = ParseArgumentDeclaration(line[beforeColNumber..], beforeColNumber, lineNumber);

                            colNumber = beforeColNumber + endCol;

                            if (IsError())
                                break;
                        }

                        // Eat whitespace
                        while (colNumber < line.Length && char.IsWhiteSpace(line[colNumber]))
                            colNumber++;
                    }

                    // Expect EOL
                    if (colNumber < line.Length)
                    {
                        AddError(new(lineNumber, colNumber, Error.Types.Syntax, string.Format("expected end of line, got '{0}'", line[colNumber])));
                        break;
                    }

                    NewKeywordNode node = new(argumentPairs)
                    {
                        LineNumber = lineNumber,
                        ColumnNumber = colNumber
                    };

                    // Nest and add a new tree
                    trees.Peek().AddNode(node);
                    trees.Push(new());
                    infoStack.Push((node, keywordInfo));

                    continue;
                }

                // Create nodes to store Before and After entities
                ASTNode? before = null, after = null;
                int beforeCol = 0, afterCol = 0;

                // Eat whitespace
                while (colNumber < line.Length && char.IsWhiteSpace(line[colNumber]))
                    colNumber++;

                if (keywordInfo.Before == TriState.YES || keywordInfo.Before == TriState.OPTIONAL)
                {
                    beforeCol = colNumber;

                    if (keywordInfo.BeforeItem == ParseOptions.Before.SYMBOL)
                    {
                        string symbolStr = ExtractSymbolFromString(line[colNumber..]);
                        if (symbolStr.Length == 0)
                        {
                            if (keywordInfo.Before == TriState.YES)
                            {
                                string got = line.Length == colNumber ? "end of line" : line[colNumber].ToString();
                                AddError(new(lineNumber, colNumber, Error.Types.Syntax, string.Format("expected symbol, got {0}", got)));
                                break;
                            }
                        }
                        else
                        {
                            SymbolNode symbolNode = new(symbolStr)
                            {
                                LineNumber = lineNumber,
                                ColumnNumber = colNumber
                            };
                            colNumber += symbolStr.Length;

                            before = symbolNode;
                        }
                    }
                    else if (keywordInfo.BeforeItem == ParseOptions.Before.CHAINED_SYMBOL)
                    {
                        (before, int endCol) = ParseSymbol(line[colNumber..], lineNumber, colNumber);
                        if (before == null)
                            break; // Propagate
                        colNumber += endCol;
                    }
                    else
                    {
                        throw new InvalidOperationException(keywordInfo.BeforeItem.ToString());
                    }
                }


                if (keywordInfo.After == TriState.YES || keywordInfo.After == TriState.OPTIONAL)
                {
                    afterCol = colNumber;

                    // Colon?
                    if (colNumber >= line.Length)
                    {
                        if (keywordInfo.After == TriState.YES)
                        {
                            AddError(new(lineNumber, colNumber, Error.Types.Syntax, "expected colon ':', got end of line"));
                            break;
                        }
                    }
                    else if (line[colNumber] == ':')
                    {
                        colNumber++;

                        while (colNumber < line.Length && char.IsWhiteSpace(line[colNumber]))
                            colNumber++;
                        afterCol = colNumber;

                        if (keywordInfo.AfterItem == ParseOptions.After.EXPR) // : Expression
                        {
                            (ExprNode? exprNode, int endCol) = ParseExpression(line[colNumber..], lineNumber, colNumber);

                            if (exprNode == null)
                            {
                                if (keywordInfo.After == TriState.YES)
                                {
                                    if (!IsError())
                                    {
                                        string got = line.Length == colNumber ? "end of line" : line[colNumber].ToString();
                                        AddError(new(lineNumber, colNumber, Error.Types.Syntax, string.Format("expected expression, got {0}", got)));
                                    }

                                    break;
                                }
                            }
                            else
                            {
                                after = exprNode;
                            }

                            colNumber += endCol;
                        }
                        else if (keywordInfo.AfterItem == ParseOptions.After.TYPE) // : Type
                        {
                            while (colNumber < line.Length && ParseNameChar.IsMatch(line[colNumber].ToString()))
                                colNumber++;

                            string s = line[afterCol..colNumber];
                            if (s.Length == 0)
                            {
                                if (keywordInfo.After == TriState.YES)
                                {
                                    string got = line.Length == colNumber ? "end of line" : line[colNumber].ToString();
                                    AddError(new(lineNumber, colNumber, Error.Types.Syntax, string.Format("expected type, got '{0}'", got)));
                                }

                                break;
                            }

                            after = new SymbolNode(s);
                        }
                        else if (keywordInfo.AfterItem == ParseOptions.After.STRING) // : "String"
                        {
                            if (line[colNumber] == '"')
                            {
                                (string str, int end) = ExtractString(line[colNumber..]);

                                if (end == -1)
                                {
                                    AddError(new(lineNumber, colNumber, Error.Types.Syntax, "unterminated string literal"));
                                    break;
                                }

                                after = new StringNode(str)
                                {
                                    LineNumber = lineNumber,
                                    ColumnNumber = colNumber
                                };
                                colNumber += end + 1;
                            }
                            else
                            {
                                string got = colNumber == line.Length ? "end of line" : line[colNumber].ToString();
                                AddError(new(lineNumber, colNumber, Error.Types.Syntax, "expected string literal, got " + got));
                                break;
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException(keywordInfo.AfterItem.ToString());
                        }
                    }
                    else
                    {
                        if (keywordInfo.After == TriState.YES)
                        {
                            AddError(new(lineNumber, colNumber, Error.Types.Syntax, string.Format("expected colon ':', got '{0}'", line[colNumber])));
                            break;
                        }
                    }
                }

                if (IsError())
                    break;

                // Must be at end of line
                if (colNumber < line.Length)
                {
                    AddError(new(lineNumber, colNumber, Error.Types.Syntax, string.Format("expected end of line, got '{0}'", line[colNumber])));
                    break;
                }

                // Create keyword node
                KeywordNode? keywordNode = null;
                bool createNewNest = false; // If true, push a new ASTStructure

                switch (keyword)
                {
                    case "CAST":
                        {
                            keywordNode = new CastKeywordNode((AbstractSymbolNode)before, new UnresolvedType(((SymbolNode)after).Symbol));
                            break;
                        }
                    case "CONST":
                        {
                            keywordNode = new ConstKeywordNode(((SymbolNode)before).Symbol, (ExprNode)after);
                            break;
                        }
                    case "DEC":
                        {
                            keywordNode = new DecKeywordNode((AbstractSymbolNode)before);
                            break;
                        }
                    case "DO":
                        {
                            if (after == null)
                            {
                                keywordNode = new DoBlockKeywordNode();
                                createNewNest = true;
                            }
                            else
                            {
                                keywordNode = new DoKeywordNode((ExprNode)after);
                            }

                            break;
                        }
                    case "ELSE":
                        {
                            if (trees.Count < 2) // Nothing to close!
                            {
                                AddError(new(lineNumber, colNumber, Error.Types.Syntax, string.Format("mismatched {0}", keyword)));
                            }
                            else
                            {
                                ASTStructure previousTree = trees.Pop();
                                ASTNode latest = trees.Peek().PeekNode();

                                if (latest is IfKeywordNode ifKeyword && ifKeyword.Conditions.Count > 0 && !ifKeyword.MetElseKeyword)
                                {
                                    ifKeyword.Conditions.Last().Body = previousTree;
                                    ifKeyword.MetElseKeyword = true;
                                }
                                else
                                {
                                    AddError(new(lineNumber, colNumber, Error.Types.Syntax, string.Format("mismatched {0}", keyword)));
                                }

                                createNewNest = true;
                            }

                            break;
                        }
                    case "ELSEIF":
                        {
                            if (trees.Count < 2) // Nothing to close!
                            {
                                AddError(new(lineNumber, keywordCol, Error.Types.Syntax, string.Format("mismatched {0}", keyword)));
                            }
                            else
                            {
                                ASTStructure previousTree = trees.Pop();
                                ASTNode latest = trees.Peek().PeekNode();

                                if (latest is IfKeywordNode ifKeyword && ifKeyword.Conditions.Count > 0)
                                {
                                    ifKeyword.Conditions.Last().Body = previousTree;
                                    ifKeyword.Conditions.Add(new((ExprNode)after));
                                }
                                else
                                {
                                    AddError(new(lineNumber, keywordCol, Error.Types.Syntax, string.Format("mismatched {0}", keyword)));
                                }

                                createNewNest = true;
                            }

                            break;
                        }
                    case "END":
                        {
                            if (trees.Count < 2) // Nothing to close!
                            {
                                AddError(new(lineNumber, keywordCol, Error.Types.Syntax, string.Format("mismatched {0}", keyword)));
                            }
                            else
                            {
                                ASTStructure previousTree = trees.Pop();
                                infoStack.Pop();
                                ASTNode latest = trees.Peek().PeekNode();

                                if (latest is IfKeywordNode ifKeyword)
                                {
                                    if (ifKeyword.MetElseKeyword)
                                    {
                                        ifKeyword.Otherwise = previousTree;
                                    }
                                    else if (ifKeyword.Conditions.Count > 0)
                                    {
                                        ifKeyword.Conditions.Last().Body = previousTree;
                                    }
                                    else
                                    {
                                        AddError(new(lineNumber, colNumber, Error.Types.Syntax, keyword));
                                    }
                                }
                                else if (latest is LoopKeywordNode loopKeyword)
                                {
                                    loopKeyword.Body = previousTree;
                                }
                                else if (latest is DefKeywordNode defKeyword)
                                {
                                    defKeyword.Body = previousTree;
                                }
                                else if (latest is NamespaceKeywordNode nsKeyword)
                                {
                                    nsKeyword.Body = previousTree;
                                }
                                else if (latest is NewKeywordNode newKeyword)
                                {
                                    newKeyword.Body = previousTree;
                                }
                                else if (latest is DoBlockKeywordNode dbKeyword)
                                {
                                    dbKeyword.Body = previousTree;
                                }
                                else if (latest is TypeKeywordNode tKeyword)
                                {
                                    tKeyword.Body = previousTree;
                                }
                                else
                                {
                                    // Should never be the case
                                    throw new InvalidOperationException(latest.ToString());
                                }
                            }

                            break;
                        }
                    case "ERROR":
                        {
                            keywordNode = new ErrorKeywordNode(after == null ? null : (ExprNode)after);
                            break;
                        }
                    case "EXIT":
                        {
                            if (trees.Count < 2)
                            {
                                AddError(new(lineNumber, keywordCol, Error.Types.Syntax, keyword));
                            }
                            else
                            {
                                keywordNode = new ExitKeywordNode();
                            }
                            break;
                        }
                    case "FIELD":
                        {
                            keywordNode = new FieldKeywordNode(((SymbolNode)before).Symbol, new UnresolvedType(((SymbolNode)after).Symbol));
                            break;
                        }
                    case "FINISH":
                        {
                            if (trees.Count < 2)
                            {
                                AddError(new(lineNumber, keywordCol, Error.Types.Syntax, keyword));
                            }
                            else
                            {
                                keywordNode = new FinishKeywordNode(after == null ? null : (ExprNode)after);
                            }
                            break;
                        }
                    case "IF":
                        {
                            IfKeywordNode ifKeyword = new();
                            ifKeyword.Conditions.Add(new((ExprNode)after));
                            keywordNode = ifKeyword;
                            createNewNest = true;
                            break;
                        }
                    case "IMPORT":
                        {
                            string path = ((StringNode)after).Value;
                            ImportKeywordNode importNode = new(path, before == null ? null : (SymbolNode)before);
                            bool isOk = importNode.Load(Options);

                            if (isOk)
                            {
                                keywordNode = importNode;
                            }
                            else
                            {
                                Options.InsertError(filename, new(lineNumber, 0, Error.Types.Import, string.Format("whilst parsing '{0}'", path)));
                            }
                            break;
                        }
                    case "INPUT":
                        {
                            keywordNode = new InputKeywordNode((AbstractSymbolNode)before);
                            break;
                        }
                    case "INC":
                        {
                            keywordNode = new IncKeywordNode((AbstractSymbolNode)before);
                            break;
                        }
                    case "LET":
                        {
                            keywordNode = new LetKeywordNode(((SymbolNode)before).Symbol, (ExprNode)after);
                            break;
                        }
                    case "LOOP":
                        {
                            LoopKeywordNode loopKeyword = new();

                            // Is there a counter?
                            if (before != null)
                            {
                                loopKeyword.Counter = ((SymbolNode)before).Symbol;
                            }

                            // Is there a condition?
                            if (after != null)
                            {
                                loopKeyword.Condition = (ExprNode)after;
                            }

                            keywordNode = loopKeyword;
                            createNewNest = true;
                            break;
                        }
                    case "NAMESPACE":
                        {
                            keywordNode = new NamespaceKeywordNode(((SymbolNode)before).Symbol);
                            createNewNest = true;
                            break;
                        }
                    case "PRINT":
                        {
                            keywordNode = new PrintKeywordNode((ExprNode)after, false);
                            break;
                        }
                    case "PRINTLN":
                        {
                            keywordNode = new PrintKeywordNode((ExprNode)after, true);
                            break;
                        }
                    case "SET":
                        {
                            keywordNode = new SetKeywordNode((AbstractSymbolNode)before, (ExprNode)after);
                            break;
                        }
                    case "STOP":
                        {
                            keywordNode = new StopKeywordNode();
                            break;
                        }
                    case "TYPE":
                        {
                            keywordNode = new TypeKeywordNode(((SymbolNode)before).Symbol);
                            createNewNest = true;
                            break;
                        }
                    default:
                        throw new InvalidOperationException(keyword);
                }

                if (IsError())
                    return;

                if (keywordNode != null)
                {
                    keywordNode.LineNumber = lineNumber;
                    if (keywordNode.ColumnNumber == 0)
                        keywordNode.ColumnNumber = startOfLineCol;

                    // Add keyword onto the current tree
                    trees.Peek().AddNode(keywordNode);
                }

                if (createNewNest)
                {
                    trees.Push(new());
                    infoStack.Push((keywordNode, keywordInfo));
                }
            }

            if (IsError())
                return;

            // End - should not be nested anymore
            if (trees.Count > 1)
            {
                AddError(new(lines.Length, 0, Error.Types.Syntax, "expected END, got end of input"));
            }
            else
            {
                AST = trees.Peek();
            }
        }

        /// <summary>
        /// Parse an argument declaration string in the form "<arg1: type1, arg2: type2, ...>". Return a list of argument names and types and the ending index. Check this.Error for if there's been an error. colStart and lineNumber are the position of str in the source.
        /// </summary>
        private (List<(string, UnresolvedType)>, int) ParseArgumentDeclaration(string str, int colStart, int lineNumber)
        {
            int col = 0;
            List<(string, UnresolvedType)> arguments = new();

            if (col == str.Length || str[col] != '<')
            {
                string got = col == str.Length ? "end of line" : str[col].ToString();
                AddError(new(lineNumber, colStart + col, Error.Types.Syntax, string.Format("expected '>', got {0}", got)));
                return (arguments, col);
            }
            col++;

            // Eat whitespace
            while (col < str.Length && char.IsWhiteSpace(str[col]))
                col++;

            // Expect "<name>: <type>"
            while (true)
            {
                // Extract argument name
                int beforeColNumber = col;
                while (col < str.Length && char.IsLetterOrDigit(str[col]))
                    col++;
                string argName = str[beforeColNumber..col];

                if (!IsValidSymbol(argName))
                {
                    AddError(new(lineNumber, colStart + beforeColNumber, Error.Types.Syntax, string.Format("invalid symbol \"{0}\"", argName)));
                    break;
                }

                // Check for duplicate names
                foreach ((string iArgName, _) in arguments)
                {
                    if (iArgName == argName)
                    {
                        AddError(new(lineNumber, colStart + beforeColNumber, Error.Types.Name, string.Format("duplicate name '{0}' in argument list", argName)));
                        break;
                    }
                }
                if (IsError())
                    break;

                // Eat whitespace
                while (col < str.Length && char.IsWhiteSpace(str[col]))
                    col++;

                // Expect colon
                if (col == str.Length || str[col] != ':')
                {
                    string got = col == str.Length ? "end of line" : str[col].ToString();
                    AddError(new(lineNumber, colStart + col, Error.Types.Syntax, string.Format("expected ':', got {0}", got)));
                    break;
                }
                col++;

                // Eat whitespace
                while (col < str.Length && char.IsWhiteSpace(str[col]))
                    col++;

                // Extract argument type
                beforeColNumber = col;
                while (col < str.Length && !(char.IsWhiteSpace(str[col]) || str[col] == '>' || str[col] == ','))
                    col++;

                string argTypeString = str[beforeColNumber..col];
                UnresolvedType argType = new(argTypeString);

                // Eat whitespace
                while (col < str.Length && char.IsWhiteSpace(str[col]))
                    col++;

                // Expect '<' or comma
                if (col == str.Length || (str[col] != '>' && str[col] != ','))
                {
                    string got = col == str.Length ? "end of line" : str[col].ToString();
                    AddError(new(lineNumber, colStart + col, Error.Types.Syntax, string.Format("expected ',' or '>', got {0}", got)));
                    break;
                }

                // Add argument to list
                arguments.Add(new(argName, argType));

                // Skip comma
                if (str[col] == ',')
                    col++;

                // Eat whitespace
                while (col < str.Length && char.IsWhiteSpace(str[col]))
                    col++;

                if (col == str.Length)
                {
                    AddError(new(lineNumber, colStart + col, Error.Types.Syntax, "expected ',' or '>', got end of line"));
                    break;
                }

                // End of argument list?
                if (str[col] == '>')
                {
                    col++;
                    break;
                }
            }

            return (arguments, col);
        }

        /// <summary>
        /// Parse a type constraint string in the form "param1: type11|type12|..., param1: type21|...". Return a dictionary of type constraints and the ending index. Check this.Error for if there's been an error. colStart and lineNumber are the position of str in the source.
        /// </summary>
        private (Dictionary<string, List<UnresolvedType>>, int) ParseTypeConstraint(string str, int colStart, int lineNumber)
        {
            int col = 0;
            Dictionary<string, List<UnresolvedType>> constraintDict = new();

            // Comma-seperated list of contraints
            while (true)
            {
                // Eat whitespace
                while (col < str.Length && char.IsWhiteSpace(str[col]))
                    col++;

                int startPos = col;
                while (col < str.Length && ParseNameChar.IsMatch(str[col].ToString()))
                    col++;
                string paramString = str[startPos..col];
                if (paramString.Length == 0)
                {
                    AddError(new(lineNumber, col + colStart, Error.Types.Syntax, "expected type parameter"));
                    break;
                }

                // Duplicate
                // TODO may be used to stack?
                if (constraintDict.ContainsKey(paramString))
                {
                    AddError(new(lineNumber, colStart + startPos, Error.Types.Type, string.Format("type {0} is already contrained", paramString)));
                    break;
                }

                // Eat whitespace
                while (col < str.Length && char.IsWhiteSpace(str[col]))
                    col++;

                // Expect colon
                if (col == str.Length || str[col] != ':')
                {
                    string got = col == str.Length ? "end of line" : str[col].ToString();
                    AddError(new(lineNumber, colStart + col, Error.Types.Syntax, string.Format("expected ':', got {0}", got)));
                    break;
                }
                col++;

                List<UnresolvedType> options = new();
                while (true)
                {
                    // Eat whitespace
                    while (col < str.Length && char.IsWhiteSpace(str[col]))
                        col++;

                    // Scan type name
                    startPos = col;
                    while (col < str.Length && ParseNameChar.IsMatch(str[col].ToString()))
                        col++;
                    string typeString = str[startPos..col];
                    if (typeString.Length == 0)
                    {
                        AddError(new(lineNumber, colStart + col, Error.Types.Syntax, "expected type in constraint clause for " + paramString));
                        break;
                    }
                    options.Add(new(typeString));

                    // Eat whitespace
                    while (col < str.Length && char.IsWhiteSpace(str[col]))
                        col++;

                    // End of clause?
                    if (col == str.Length || str[col] != '|')
                        break;
                    col++;
                }

                if (IsError())
                    break;

                constraintDict.Add(paramString, options);

                // End of contraint, or is there another?
                if (col == str.Length || str[col] != ',')
                    break;
                col++;
            }

            return (constraintDict, col);
        }

        /// <summary>
        /// Return whether or not the provided string is a valid symbol
        /// </summary>
        public static bool IsValidSymbol(string symbol)
        {
            return SymbolRegex.IsMatch(symbol);
        }

        private static readonly Regex SymbolRegex = new("^[A-Za-z_\\$][A-Za-z_\\$0-9\\.]*$");
        private static readonly Regex LeadingSymbolCharRegex = new("[A-Za-z_\\$]");
        private static readonly Regex LeadingSymbolRegex = new("^(?<symbol>[A-Za-z_\\$][A-Za-z_\\$0-9]*)");
        private static readonly Regex FloatRegex = new("^(?<number>-?(0|[1-9]\\d*)(\\.\\d+)?)");
        private static readonly Regex IntegerRegex = new("^(?<integer>-?(0|[1-9]\\d*))");
        private static readonly Regex ParseNameChar = new("[A-Za-z\\[\\]0-9]");
        public static readonly Regex NonWhitespaceRegex = new("[^\\s]");

        /// <summary>
        /// Extract the leading symbol from the given string
        /// </summary>
        private static string ExtractSymbolFromString(string str)
        {
            Match match = LeadingSymbolRegex.Match(str);
            return match.Groups["symbol"].Value;
        }

        /// <summary>
        /// Extract a decimal number from a string
        /// </summary>
        private static string ExtractFloatFromString(string str)
        {
            Match match = FloatRegex.Match(str);
            return match.Groups["number"].Value;
        }

        /// <summary>
        /// Extract an integer from a string
        /// </summary>
        private static string ExtractIntegerFromString(string str)
        {
            Match match = IntegerRegex.Match(str);
            return match.Groups["integer"].Value;
        }

        private (List<ExprNode>?, int) ParseSymbolArguments(string str, int lineNumber = 0, int colNumber = 0)
        {
            int col = 0;
            List<ExprNode> arguments = new();

            // Were any arguments provided?
            if (col < str.Length && str[col] == '<')
            {
                int argStartPos = col;
                col++;

                // Extract each argument, seperated by ','
                while (col < str.Length)
                {
                    if (str[col] == '>')
                        break;

                    (ExprNode? argExpr, int end) = ParseExpression(str[col..], lineNumber, colNumber + col, new char?[] { ',', '>', null });

                    if (argExpr == null)
                    {
                        return (null, col);
                    }
                    else
                    {
                        col += end;
                        arguments.Add(argExpr);
                        if (str[col] == '>')
                            break;
                        col++;
                    }
                }

                if (col == str.Length)
                {
                    AddError(new(lineNumber, colNumber + col, Error.Types.Syntax, "expected '>', got end of line"));
                    return (null, col);
                }
                else if (str[col] == '>')
                {
                    col++;
                }
                else
                {
                    AddError(new(lineNumber, colNumber + col, Error.Types.Syntax, string.Format("expected '>', got {0}", str[col])));
                    return (null, col);
                }
            }

            return (arguments, col);
        }

        /// <summary>
        /// Parse a symbol. Could be either a single symbol (SymbolNode), or an access chain (ChainedSymbolNode). Unlike in expressions, each property cannot be an expression itself.
        /// </summary>
        private (AbstractSymbolNode?, int) ParseSymbol(string expr, int lineNumber = 0, int colNumber = 0)
        {
            int col = 0;

            // Extract symbol
            string symbol = ExtractSymbolFromString(expr);
            if (symbol.Length == 0)
            {
                AddError(new(lineNumber, colNumber, Error.Types.Syntax, "expected symbol, got " + expr[col]));
                return (null, col);
            }

            // Create base symbol node
            SymbolNode baseSymbolNode = new(symbol)
            {
                LineNumber = lineNumber,
                ColumnNumber = colNumber + col
            };

            col += symbol.Length;


            // Eat whitespace
            while (col < expr.Length && char.IsWhiteSpace(expr[col]))
                col++;

            // Arguments?
            (List<ExprNode>? arguments, int endCol) = ParseSymbolArguments(expr[col..], lineNumber, col + colNumber);
            if (arguments == null)
                return (null, col + endCol);

            col += endCol;
            baseSymbolNode.CallArguments = arguments;

            // Property chain?
            if (col < expr.Length && expr[col] == '.')
            {
                ChainedSymbolNode chainNode = new();
                chainNode.Components.Add(baseSymbolNode);

                while (col < expr.Length && expr[col] == '.')
                {
                    col++;
                    SymbolNode childNode;

                    // Numerical property?
                    if (char.IsDigit(expr[col]) || (expr[col] == '-' && col + 1 < expr.Length && char.IsDigit(expr[col + 1])))
                    {
                        string str = ExtractIntegerFromString(expr[col..]);
                        childNode = new(str)
                        {
                            ColumnNumber = col + colNumber,
                            LineNumber = lineNumber
                        };

                        col += str.Length;
                    }
                    else
                    {
                        // Extract symbol
                        symbol = ExtractSymbolFromString(expr[col..]);
                        if (symbol.Length == 0)
                        {
                            string got = col == expr.Length ? "end of line" : expr[col].ToString();
                            AddError(new(lineNumber, colNumber + col, Error.Types.Syntax, "expected symbol, got " + got));
                            return (null, col);
                        }

                        childNode = new(symbol)
                        {
                            ColumnNumber = col + colNumber,
                            LineNumber = lineNumber
                        };

                        col += symbol.Length;

                        // Eat whitespace
                        while (col < expr.Length && char.IsWhiteSpace(expr[col]))
                            col++;

                        // Arguments?
                        (arguments, endCol) = ParseSymbolArguments(expr[col..], lineNumber, col + colNumber);
                        if (arguments == null)
                            return (null, col + endCol);

                        col += endCol;
                        childNode.CallArguments = arguments;
                    }

                    chainNode.Components.Add(childNode);
                }

                return (chainNode, col);
            }
            else
            {
                return (baseSymbolNode, col);
            }
        }

        private class NestedExpr
        {
            public ExprNode Expr = new();
            public bool UpdatedPropertyAccess = false; // Have we updated EncounteredPropertyAccess this iteration?
            public bool EncounteredPropertyAccess; // Encountered a dot '.'?

            public NestedExpr(bool encounteredPropertyAccess)
            {
                EncounteredPropertyAccess = encounteredPropertyAccess;
            }
        }

        /// <summary>
        /// Parse a string as an expresion. Return the expression node and the ending index. The expression must terminate with any character in endChar (NULL means that the line must end)
        /// </summary>
        private (ExprNode?, int) ParseExpression(string expr, int lineNumber = 0, int colNumber = 0, char?[]? endChar = null)
        {
            int col = 0;
            Stack<NestedExpr> exprStack = new();
            exprStack.Push(new(false));

            // Eat whitespace
            while (col < expr.Length && char.IsWhiteSpace(expr[col]))
                col++;

            while (true)
            {
                exprStack.Peek().UpdatedPropertyAccess = false;
                int startLoopPos = col;

                // Is end of the line?
                if (col == expr.Length)
                {
                    AddError(new(lineNumber, colNumber + col, Error.Types.Syntax, "unexpected end of line"));
                    return (null, col);
                }

                ASTNode? node = null;
                int startPos = col;

                // Is a string literal?
                if (expr[col] == '"')
                {
                    (string str, int end) = ExtractString(expr[col..]);

                    if (end == -1)
                    {
                        AddError(new(lineNumber, colNumber + col, Error.Types.Syntax, "unterminated string literal"));
                        return (null, col);
                    }

                    node = new ValueNode(new StringValue(str))
                    {
                        LineNumber = lineNumber,
                        ColumnNumber = colNumber + col
                    };
                    col += end + 1;
                }

                // Is a number?
                else if (char.IsDigit(expr[col]) || (expr[col] == '-' && col + 1 < expr.Length && char.IsDigit(expr[col + 1])))
                {
                    string str = ExtractFloatFromString(expr[col..]);
                    double number = Convert.ToDouble(str);
                    Value value;

                    if (number == (long)number)
                    {
                        value = new IntValue((long)number);
                    }
                    else
                    {
                        value = new FloatValue(number);
                    }

                    node = new ValueNode(value)
                    {
                        LineNumber = lineNumber,
                        ColumnNumber = colNumber + col
                    };
                    col += str.Length;

                    NestedExpr latest = exprStack.Peek();
                    if (latest.EncounteredPropertyAccess)
                    {
                        ((ChainedSymbolNode)latest.Expr.Children[^1]).Components.Add(node);
                        node = null;
                        latest.EncounteredPropertyAccess = false;
                    }
                }

                // Is type literal
                else if (expr[col] == TypeLiteralChar)
                {
                    col++;

                    if (col == expr.Length)
                    {
                        AddError(new(lineNumber, colNumber + col, Error.Types.Syntax, "unexpected end of line"));
                        return (null, col);
                    }

                    startPos = col;
                    while (col < expr.Length && ParseNameChar.IsMatch(expr[col].ToString()))
                        col++;

                    string s = expr[startPos..col];
                    node = new TypeNode(new(s))
                    {
                        LineNumber = lineNumber,
                        ColumnNumber = colNumber + startPos
                    };
                }

                // Type casting?
                else if (expr[col] == '(')
                {
                    ExprNode exprNode = exprStack.Peek().Expr;

                    // Already a type casting?
                    if (exprNode.CastType == null)
                    {
                        int end = GetMatchingClosingItem(expr[col..], '(', ')');
                        if (end == -1)
                        {
                            AddError(new(lineNumber, colNumber + col, Error.Types.Syntax, string.Format("unterminated bracket '{0}'", expr[col])));
                            return (null, col);
                        }

                        string str = expr[(col + 1)..(col + end)];
                        UnresolvedType type = new(str);
                        exprNode.CastType = type;
                        col += end + 1;
                    }
                    else
                    {
                        AddError(new(lineNumber, colNumber + col, Error.Types.Syntax, "unexpected '(': type cast already encountered for this expression (" + exprNode.CastType.Value.GetSymbolString() + ")"));
                        return (null, col);
                    }
                }

                // Precedence grouping?
                else if (expr[col] == '[')
                {
                    exprStack.Push(new(false)
                    {
                        Expr = new()
                        {
                            LineNumber = lineNumber,
                            ColumnNumber = colNumber + col
                        }
                    });

                    col++;
                }
                else if (expr[col] == ']')
                {
                    if (exprStack.Count > 1)
                    {
                        col++;
                        NestedExpr old = exprStack.Pop();
                        if (old.EncounteredPropertyAccess)
                        {
                            AddError(new(lineNumber, colNumber + col, Error.Types.Syntax, "unexpected ']' after '.'"));
                            return (null, col);
                        }
                        if (old.Expr.Children.Count == 0)
                        {
                            AddError(new(lineNumber, colNumber + col, Error.Types.Syntax, "unexpected ']' in nested expression"));
                            return (null, col);
                        }

                        NestedExpr latest = exprStack.Peek();

                        if (latest.EncounteredPropertyAccess)
                        {
                            ((ChainedSymbolNode)latest.Expr.Children[^1]).Components.Add(old.Expr);
                            latest.EncounteredPropertyAccess = false;
                        }
                        else
                        {
                            latest.Expr.Children.Add(old.Expr);
                        }
                    }
                    else
                    {
                        AddError(new(lineNumber, colNumber + col, Error.Types.Syntax, string.Format("expected ']', got '{0}'", expr[col])));
                        return (null, col);
                    }
                }

                // Property access?
                else if (expr[col] == '.')
                {
                    NestedExpr state = exprStack.Peek();
                    if (state.EncounteredPropertyAccess || state.Expr.Children.Count == 0)
                    {
                        AddError(new(lineNumber, colNumber + col, Error.Types.Syntax, "unexpected '.'"));
                        return (null, col);
                    }

                    ASTNode latest = state.Expr.Children[^1];
                    if (latest is not ChainedSymbolNode)
                    {
                        ChainedSymbolNode chain = new();
                        chain.Components.Add(latest);
                        state.Expr.Children[^1] = chain;
                    }

                    col++;
                    state.EncounteredPropertyAccess = true;
                    state.UpdatedPropertyAccess = true;
                }

                // List constructor?
                else if (expr[col] == '{')
                {
                    ListConstructNode listNode;

                    // Type node before?
                    ExprNode prev = exprStack.Peek().Expr;
                    if (prev.Children.Count > 0 && prev.Children[^1] is TypeNode tNode)
                    {
                        listNode = new(tNode.Value)
                        {
                            ColumnNumber = tNode.ColumnNumber,
                            LineNumber = tNode.LineNumber,
                        };

                        prev.Children.RemoveAt(prev.Children.Count - 1);
                    }
                    else
                    {
                        listNode = new()
                        {
                            ColumnNumber = colNumber + col,
                            LineNumber = lineNumber
                        };
                    }

                    col++;
                    startPos = col;

                    // Eat whitespace
                    while (col < expr.Length && char.IsWhiteSpace(expr[col]))
                        col++;

                    // Ending brace? Or arguments?
                    if (col < expr.Length && expr[col] == '}')
                    {
                        if (listNode.ListType == null)
                        {
                            AddError(new(lineNumber, colNumber + startPos, Error.Types.Syntax, "unexpected '{' (cannot construct empty list of unknown type)"));
                            return (null, col);
                        }
                        else
                        {
                            col++;
                        }
                    }
                    else
                    {
                        // Extract each argument, seperated by ','
                        while (col < expr.Length)
                        {
                            (ExprNode? argExpr, int end) = ParseExpression(expr[col..], lineNumber, colNumber + col, new char?[] { ',', '}', null });

                            if (argExpr == null)
                            {
                                return (null, col);
                            }
                            else
                            {
                                col += end;
                                listNode.Members.Add(argExpr);
                                if (expr[col] == '}')
                                    break;
                                col++;
                            }
                        }

                        if (col == expr.Length)
                        {
                            AddError(new(lineNumber, colNumber + col, Error.Types.Syntax, "expected '}', got end of line"));
                            return (null, col);
                        }
                        else if (expr[col] == '}')
                        {
                            col++;
                        }
                        else
                        {
                            AddError(new(lineNumber, colNumber + col, Error.Types.Syntax, string.Format("expected '}}', got {0}", expr[col])));
                            return (null, col);
                        }
                    }

                    node = listNode;
                }

                // Function arguments?
                else if (expr[col] == '<')
                {
                    ExprNode exprNode = exprStack.Peek().Expr;
                    if (exprNode.Children.Count == 0 || (exprNode.Children[^1] is not AbstractSymbolNode && exprNode.Children[^1] is not TypeNode))
                    {
                        AddError(new(lineNumber, colNumber + col, Error.Types.Syntax, "unexpected '<' (must be preceeded by a symbol)"));
                        return (null, col);
                    }

                    if (exprNode.Children[^1] is AbstractSymbolNode aSymbolNode)
                    {
                        // Retrieve the SymbolNode instance which the arguments which attach to
                        SymbolNode symbolNode;
                        if (aSymbolNode is SymbolNode symbol)
                            symbolNode = symbol;
                        else if (aSymbolNode is ChainedSymbolNode chain)
                        {
                            if (chain.Components.Count == 0 || chain.Components[^1] is not SymbolNode)
                            {
                                AddError(new(lineNumber, colNumber + col, Error.Types.Syntax, "unexpected '<' (must be preceeded by a symbol)"));
                                return (null, col);
                            }
                            else
                            {
                                symbolNode = (SymbolNode)chain.Components[^1];
                            }
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }

                        if (symbolNode.CallArguments == null)
                        {
                            // Extract the arguments
                            (List<ExprNode>? arguments, int endCol) = ParseSymbolArguments(expr[col..], lineNumber, colNumber + col);
                            col += endCol;

                            if (arguments == null)
                                return (null, col);

                            symbolNode.CallArguments = arguments;
                        }
                        else
                        {
                            AddError(new(lineNumber, colNumber + col, Error.Types.Syntax, "unexpected '<' (must be preceeded by a symbol)"));
                            return (null, col);
                        }
                    }
                    else if (exprNode.Children[^1] is TypeNode typeNode)
                    {
                        TypeConstructNode constructNode = new(typeNode.Value)
                        {
                            LineNumber = typeNode.LineNumber,
                            ColumnNumber = typeNode.ColumnNumber
                        };

                        // Extract the arguments
                        (List<ExprNode>? arguments, int endCol) = ParseSymbolArguments(expr[col..], lineNumber, colNumber + col);
                        col += endCol;

                        if (arguments == null)
                            return (null, col);

                        constructNode.Arguments = arguments;
                        exprNode.Children[^1] = constructNode;
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }

                // Parse as a symbol
                else
                {
                    int startCol = col;
                    string symbol = ExtractSymbolFromString(expr[startCol..]);
                    if (symbol.Length == 0)
                    {
                        string got = startCol == expr.Length ? "end of line" : expr[col].ToString();
                        AddError(new(lineNumber, colNumber + startCol, Error.Types.Syntax, string.Format("expected symbol, got {0}", got)));
                        return (null, startCol);
                    }
                    col += symbol.Length;

                    SymbolNode symbolNode = new(symbol)
                    {
                        LineNumber = lineNumber,
                        ColumnNumber = colNumber + startCol
                    };

                    NestedExpr state = exprStack.Peek();
                    if (state.EncounteredPropertyAccess)
                    {
                        ((ChainedSymbolNode)state.Expr.Children[^1]).Components.Add(symbolNode);
                        state.EncounteredPropertyAccess = false;
                    }
                    else
                    {
                        node = symbolNode;
                    }
                }

                // Check property access
                NestedExpr nest = exprStack.Peek();
                if (!nest.UpdatedPropertyAccess)
                {
                    if (nest.EncounteredPropertyAccess)
                    {
                        col = startLoopPos;
                        string got = col == expr.Length ? "end of line" : expr[col].ToString();
                        AddError(new(lineNumber, colNumber + col, Error.Types.Syntax, string.Format("unexpected '{0}' following '.'", got)));
                        return (null, col);
                    }
                }

                // Eat whitespace
                while (col < expr.Length && char.IsWhiteSpace(expr[col]))
                    col++;

                if (node != null)
                {
                    nest.Expr.Children.Add(node);
                }

                bool doBreak = false;

                // Stop if: Reached the end of the line? Comment? Bracket? Met and ending character?
                if (col == expr.Length || (endChar != null && endChar.Contains(expr[col])))
                    doBreak = true;
                else if (expr[col] == CommentChar)
                {
                    endChar = null;
                    col = expr.Length;
                    doBreak = true;
                }

                if (doBreak)
                {
                    if (nest.EncounteredPropertyAccess)
                    {
                        col = startLoopPos;
                        string got = col == expr.Length ? "end of line" : expr[col].ToString();
                        AddError(new(lineNumber, colNumber + col, Error.Types.Syntax, string.Format("expected symbol, number, string or '[', got '{0}'", got)));
                        return (null, col);
                    }

                    break;
                }
            }

            // Eat whitespace
            while (col < expr.Length && char.IsWhiteSpace(expr[col]))
                col++;

            // The input string should end with `endChar` or a comment
            if (col < expr.Length)
            {
                if (expr[col] == CommentChar)
                {

                }
                else if (endChar == null)
                {
                    AddError(new(lineNumber, colNumber + col, Error.Types.Syntax, string.Format("expected end of line, got {0}", expr[col])));
                    return (null, col);
                }
                else if (endChar.Contains(expr[col]))
                {

                }
                else
                {
                    AddError(new(lineNumber, colNumber + col, Error.Types.Syntax, string.Format("expected {0}, got {1}", string.Join(" or ", endChar.Select(a => a == null ? "end of line" : a.ToString())), expr[col])));
                    return (null, col);
                }
            }
            else
            {
                if (endChar != null && !endChar.Contains(null))
                {
                    AddError(new(lineNumber, colNumber + col, Error.Types.Syntax, "unexpected end of line"));
                    return (null, col);
                }
            }

            if (exprStack.Count == 1)
            {
                return (exprStack.Peek().Expr, col);
            }
            else
            {
                string got = col == expr.Length ? "end of line" : expr[col].ToString();
                AddError(new(lineNumber, colNumber + col, Error.Types.Syntax, string.Format("expected ']', got {0}", got)));
                return (null, col);
            }
        }

        /// <summary>
        /// Given a string, return the index of the closing <paramref name="close"/> character. Return -1 of closing character not found.
        /// </summary>
        private static int GetMatchingClosingItem(string str, char open, char close)
        {
            for (int i = 0, c = 0; i < str.Length; i++)
            {
                if (str[i] == open)
                {
                    c++;
                }
                else if (str[i] == close)
                {
                    c--;
                    if (c == 0)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Return the extracted string and the ending index of said string
        /// </summary>
        private static (string, int) ExtractString(string str)
        {
            int i = str[0] == '"' ? 1 : 0;
            string extract = "";
            while (i < str.Length && str[i] != '"')
            {
                extract += str[i++];
            }

            if (i < str.Length && str[i] == '"')
            {
                return (extract, i);
            }
            else
            {
                return ("", -1);
            }
        }

        public bool IsError()
        {
            return Options.IsError();
        }

        public string GetErrorString()
        {
            return Options.GetErrorString();
        }

        public ASTStructure GetAST()
        {
            return AST ?? throw new NullReferenceException();
        }
    }
}
