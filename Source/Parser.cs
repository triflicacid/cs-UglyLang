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

        public Error? Error = null;
        public ASTStructure? AST = null;
        public string Source = "";

        public void Parse(string program)
        {
            AST = null;
            Error = null;
            Source = program;

            // Nested structure
            Stack<ASTStructure> trees = new();
            trees.Push(new());
            bool inComment = false;

            string[] lines = program.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            for (int lineNumber = 0, colNumber = 0; lineNumber < lines.Length; lineNumber++, colNumber = 0)
            {
                string line = lines[lineNumber];

                // Eat whitespace
                while (colNumber < line.Length && char.IsWhiteSpace(line[colNumber])) colNumber++;

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
                    Error = new(lineNumber, colNumber, Error.Types.Syntax, string.Format("expected keyword, got '{0}'", line[colNumber]));
                    break;
                }
                else if (!KeywordNode.KeywordDict.ContainsKey(keyword))
                {
                    Error = new(lineNumber, colNumber, Error.Types.Syntax, keyword);
                    break;
                }

                // Is define keyword? This one needs special parsing.
                if (keyword == "DEF")
                {
                    // Eat whitespace
                    while (colNumber < line.Length && char.IsWhiteSpace(line[colNumber])) colNumber++;

                    // Extract name
                    int beforeColNumber = colNumber;
                    while (colNumber < line.Length && char.IsLetterOrDigit(line[colNumber])) colNumber++;
                    string functionName = line[beforeColNumber..colNumber];

                    if (!IsValidSymbol(functionName))
                    {
                        Error = new(lineNumber, beforeColNumber, Error.Types.Syntax, string.Format("invalid symbol \"{0}\"", functionName));
                        break;
                    }

                    // Eat whitespace
                    while (colNumber < line.Length && char.IsWhiteSpace(line[colNumber])) colNumber++;

                    // Should be a colon
                    if (colNumber == line.Length || line[colNumber] != ':')
                    {
                        string got = colNumber == line.Length ? "end of line" : line[colNumber].ToString();
                        Error = new(lineNumber, colNumber, Error.Types.Syntax, string.Format("expected ':', got {0}", got));
                        break;
                    }
                    colNumber++;

                    // Eat whitespace
                    while (colNumber < line.Length && char.IsWhiteSpace(line[colNumber])) colNumber++;

                    beforeColNumber = colNumber;

                    // Return type
                    UnresolvedType returnType;
                    if (colNumber < line.Length && line[colNumber] == '<')
                    {
                        returnType = new ResolvedType(new EmptyType()); // No return type
                    }
                    else
                    {
                        while (colNumber < line.Length && !char.IsWhiteSpace(line[colNumber]))
                            colNumber++;

                        string returnTypeString = line[beforeColNumber..colNumber];
                        returnType = new UnresolvedType(returnTypeString);
                    }

                    // Eat whitespace
                    while (colNumber < line.Length && char.IsWhiteSpace(line[colNumber])) colNumber++;

                    // Expect angled bracket
                    beforeColNumber = colNumber;
                    if (colNumber == line.Length || line[colNumber] != '<')
                    {
                        string got = colNumber == line.Length ? "end of line" : line[colNumber].ToString();
                        Error = new(lineNumber, colNumber, Error.Types.Syntax, string.Format("expected '<', got {0}", got));
                        break;
                    }
                    colNumber++;

                    // Eat whitespace
                    while (colNumber < line.Length && char.IsWhiteSpace(line[colNumber])) colNumber++;

                    if (colNumber == line.Length)
                    {
                        Error = new(lineNumber, colNumber, Error.Types.Syntax, "expected '>' or symbol, got end of line");
                        break;
                    }


                    List<(string, UnresolvedType)> argumentPairs;
                    if (line[colNumber] == '>')
                    {
                        colNumber++;
                        argumentPairs = new();
                    }
                    else
                    {
                        (argumentPairs, int endCol) = ParseArgumentDeclaration(line[beforeColNumber..], beforeColNumber, lineNumber);

                        colNumber = beforeColNumber + endCol;

                        if (Error != null)
                            break;
                    }

                    // Eat whitespace
                    while (colNumber < line.Length && char.IsWhiteSpace(line[colNumber]))
                        colNumber++;

                    // "WHERE" contraint clause?
                    Dictionary<string, List<UnresolvedType>>? constraints = null;
                    if (line[colNumber..].StartsWith("WHERE"))
                    {
                        // Eat non-whitespace to skip keyword
                        while (colNumber < line.Length && !char.IsWhiteSpace(line[colNumber]))
                            colNumber++;

                        (constraints, int endCol) = ParseTypeConstraint(line[colNumber..], colNumber, lineNumber);
                        colNumber += endCol;

                        if (Error != null)
                            break;
                    }

                    // Create keyword node and add to tree structure
                    DefKeywordNode node = new(functionName, argumentPairs, returnType, constraints);
                    trees.Peek().AddNode(node);

                    // Should be at the end of the line
                    if (colNumber < line.Length)
                    {
                        Error = new(lineNumber, colNumber, Error.Types.Syntax, string.Format("expected end of line, got '{0}'", line[colNumber]));
                        break;
                    }

                    // Nest and add a new tree
                    trees.Push(new());

                    continue;
                }

                // Fetch keyword information - this will tell us what to parse.
                var keywordInfo = KeywordNode.KeywordDict[keyword];
                ASTNode? before = null, after = null;
                int beforeCol = 0, afterCol = 0;

                if (keywordInfo != null)
                {
                    // Eat whitespace
                    while (colNumber < line.Length && char.IsWhiteSpace(line[colNumber])) colNumber++;

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
                                    Error = new(lineNumber, colNumber, Error.Types.Syntax, string.Format("expected symbol, got {0}", got));
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
                            if (before == null) break; // Propagate
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
                                Error = new(lineNumber, colNumber, Error.Types.Syntax, "expected colon ':', got end of line");
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
                                        if (Error == null)
                                        {
                                            string got = line.Length == colNumber ? "end of line" : line[colNumber].ToString();
                                            Error = new(lineNumber, colNumber, Error.Types.Syntax, string.Format("expected expression, got {0}", got));
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
                                while (colNumber < line.Length && ParseNameChar.IsMatch(line[colNumber].ToString())) colNumber++;

                                string s = line[afterCol..colNumber];
                                if (s.Length == 0)
                                {
                                    if (keywordInfo.After == TriState.YES)
                                    {
                                        string got = line.Length == colNumber ? "end of line" : line[colNumber].ToString();
                                        Error = new(lineNumber, colNumber, Error.Types.Syntax, string.Format("expected type, got '{0}'", got));
                                    }

                                    break;
                                }

                                after = new SymbolNode(s);
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
                                Error = new(lineNumber, colNumber, Error.Types.Syntax, string.Format("expected colon ':', got '{0}'", line[colNumber]));
                                break;
                            }
                        }
                    }
                }

                if (Error != null) break;

                // Must be at end of line
                if (colNumber < line.Length)
                {
                    Error = new(lineNumber, colNumber, Error.Types.Syntax, string.Format("expected end of line, got '{0}'", line[colNumber]));
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
                    case "DO":
                        {
                            keywordNode = new DoKeywordNode((ExprNode)after);
                            break;
                        }
                    case "ELSE":
                        {
                            if (trees.Count < 2) // Nothing to close!
                            {
                                Error = new(lineNumber, colNumber, Error.Types.Syntax, string.Format("mismatched {0}", keyword));
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
                                    Error = new(lineNumber, colNumber, Error.Types.Syntax, string.Format("mismatched {0}", keyword));
                                }

                                createNewNest = true;
                            }

                            break;
                        }
                    case "ELSEIF":
                        {
                            if (trees.Count < 2) // Nothing to close!
                            {
                                Error = new(lineNumber, keywordCol, Error.Types.Syntax, string.Format("mismatched {0}", keyword));
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
                                    Error = new(lineNumber, keywordCol, Error.Types.Syntax, string.Format("mismatched {0}", keyword));
                                }

                                createNewNest = true;
                            }

                            break;
                        }
                    case "END":
                        {
                            if (trees.Count < 2) // Nothing to close!
                            {
                                Error = new(lineNumber, keywordCol, Error.Types.Syntax, string.Format("mismatched {0}", keyword));
                            }
                            else
                            {
                                ASTStructure previousTree = trees.Pop();
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
                                        Error = new(lineNumber, colNumber, Error.Types.Syntax, keyword);
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
                                Error = new(lineNumber, keywordCol, Error.Types.Syntax, keyword);
                            }
                            else
                            {
                                keywordNode = new ExitKeywordNode();
                            }
                            break;
                        }
                    case "FINISH":
                        {
                            if (trees.Count < 2)
                            {
                                Error = new(lineNumber, keywordCol, Error.Types.Syntax, keyword);
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
                    case "INPUT":
                        {
                            keywordNode = new InputKeywordNode((AbstractSymbolNode)before);
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
                    default:
                        throw new InvalidOperationException();
                }

                if (Error != null) return;

                if (keywordNode != null)
                {
                    keywordNode.LineNumber = lineNumber;

                    // Add keyword onto the current tree
                    trees.Peek().AddNode(keywordNode);
                }

                if (createNewNest)
                {
                    trees.Push(new());
                }
            }

            if (Error != null) return;

            // End - should not be nested anymore
            if (trees.Count > 1)
            {
                Error = new(lines.Length, 0, Error.Types.Syntax, "expected END, got end of input");
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
                Error = new(lineNumber, colStart + col, Error.Types.Syntax, string.Format("expected '>', got {0}", got));
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
                    Error = new(lineNumber, colStart + beforeColNumber, Error.Types.Syntax, string.Format("invalid symbol \"{0}\"", argName));
                    break;
                }

                // Check for duplicate names
                foreach ((string iArgName, _) in arguments)
                {
                    if (iArgName == argName)
                    {
                        Error = new(lineNumber, colStart + beforeColNumber, Error.Types.Name, string.Format("duplicate name '{0}' in argument list", argName));
                        break;
                    }
                }
                if (Error != null)
                    break;

                // Eat whitespace
                while (col < str.Length && char.IsWhiteSpace(str[col]))
                    col++;

                // Expect colon
                if (col == str.Length || str[col] != ':')
                {
                    string got = col == str.Length ? "end of line" : str[col].ToString();
                    Error = new(lineNumber, colStart + col, Error.Types.Syntax, string.Format("expected ':', got {0}", got));
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
                    Error = new(lineNumber, colStart + col, Error.Types.Syntax, string.Format("expected ',' or '>', got {0}", got));
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
                    Error = new(lineNumber, colStart + col, Error.Types.Syntax, "expected ',' or '>', got end of line");
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
                    Error = new(lineNumber, col + colStart, Error.Types.Syntax, "expected type parameter");
                    break;
                }

                // Duplicate
                // TODO may be used to stack?
                if (constraintDict.ContainsKey(paramString))
                {
                    Error = new(lineNumber, colStart + startPos, Error.Types.Type, string.Format("type {0} is already contrained", paramString));
                    break;
                }

                // Eat whitespace
                while (col < str.Length && char.IsWhiteSpace(str[col]))
                    col++;

                // Expect colon
                if (col == str.Length || str[col] != ':')
                {
                    string got = col == str.Length ? "end of line" : str[col].ToString();
                    Error = new(lineNumber, colStart + col, Error.Types.Syntax, string.Format("expected ':', got {0}", got));
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
                        Error = new(lineNumber, colStart + col, Error.Types.Syntax, "expected type in constraint clause for " + paramString);
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

                if (Error != null)
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
                    Error = new(lineNumber, colNumber + col, Error.Types.Syntax, "expected '>', got end of line");
                    return (null, col);
                }
                else if (str[col] == '>')
                {
                    col++;
                }
                else
                {
                    Error = new(lineNumber, colNumber + col, Error.Types.Syntax, string.Format("expected '>', got {0}", str[col]));
                    return (null, col);
                }
            }

            return (arguments, col);
        }

        /// <summary>
        /// Parse a symbol. Could be either a single symbol (SymbolNode), or an access chain (ChainedSymbolNode).
        /// </summary>
        private (AbstractSymbolNode?, int) ParseSymbol(string expr, int lineNumber = 0, int colNumber = 0)
        {
            int col = 0;

            // Extract symbol
            string symbol = ExtractSymbolFromString(expr);
            if (symbol.Length == 0)
            {
                Error = new(lineNumber, colNumber, Error.Types.Syntax, "expected symbol, got " + expr[col]);
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
                chainNode.Symbols.Add(baseSymbolNode);

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
                            Error = new(lineNumber, colNumber + col, Error.Types.Syntax, "expected symbol, got " + got);
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

                    chainNode.Symbols.Add(childNode);
                }

                return (chainNode, col);
            }
            else
            {
                return (baseSymbolNode, col);
            }
        }

        /// <summary>
        /// Parse a string as an expresion. Return the expression node and the ending index. The expression must terminate with any character in endChar (NULL means that the line must end)
        /// </summary>
        private (ExprNode?, int) ParseExpression(string expr, int lineNumber = 0, int colNumber = 0, char?[]? endChar = null)
        {
            int col = 0;
            ExprNode exprNode = new();

            // Eat whitespace
            while (col < expr.Length && char.IsWhiteSpace(expr[col])) col++;

            while (true)
            {
                // Is end of the line?
                if (col == expr.Length)
                {
                    Error = new(lineNumber, colNumber + col, Error.Types.Syntax, "unexpected end of line");
                    return (null, col);
                }

                ASTNode? node;
                int startPos = col;

                // Is a string literal?
                if (expr[col] == '"')
                {
                    (string str, int end) = ExtractString(expr[col..]);

                    if (end == -1)
                    {
                        Error = new(lineNumber, colNumber + col, Error.Types.Syntax, "unterminated string literal");
                        return (null, col);
                    }

                    node = new ValueNode(new StringValue(str));
                    node.ColumnNumber = colNumber + col;
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

                    node = new ValueNode(value);
                    node.ColumnNumber = colNumber + col;
                    col += str.Length;
                }

                // Is type literal
                else if (expr[col] == TypeLiteralChar)
                {
                    col++;

                    if (col == expr.Length)
                    {
                        Error = new(lineNumber, colNumber + col, Error.Types.Syntax, "unexpected end of line");
                        return (null, col);
                    }

                    startPos = col;
                    while (col < expr.Length && ParseNameChar.IsMatch(expr[col].ToString()))
                        col++;

                    string s = expr[startPos..col];
                    node = new TypeNode(new(s));
                    node.ColumnNumber = colNumber + startPos;
                }

                // Extract the next word and proceed from there
                else
                {
                    // Extract symbol, then extract all until whitespace
                    startPos = col;
                    while (col < expr.Length && ParseNameChar.IsMatch(expr[col].ToString())) col++;
                    string str = expr[startPos..col];
                    bool isTypeless = str.Length == 0; // If typeless, assume list

                    // Eat whitespace
                    while (col < expr.Length && char.IsWhiteSpace(expr[col])) col++;

                    // If there is a brace, it is a type constructor, else it is a symbol
                    if (col < expr.Length && expr[col] == '{')
                    {
                        TypeConstructNode typeNode = new(isTypeless ? null : new(str));
                        typeNode.ColumnNumber = colNumber + startPos;

                        col++;
                        startPos = col;

                        // Eat whitespace
                        while (col < expr.Length && char.IsWhiteSpace(expr[col])) col++;

                        // End?
                        if (col < expr.Length && expr[col] == '}')
                        {
                            if (isTypeless) // We cannot determine the type as no arguments where provided
                            {
                                Error = new(lineNumber, colNumber + startPos, Error.Types.Syntax, "expected type name, got '{'");
                                return (null, col);
                            }

                            col++;
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
                                    typeNode.Arguments.Add(argExpr);
                                    if (expr[col] == '}') break;
                                    col++;
                                }
                            }

                            if (col == expr.Length)
                            {
                                Error = new(lineNumber, colNumber + col, Error.Types.Syntax, "expected '}', got end of line");
                                return (null, col);
                            }
                            else if (expr[col] == '}')
                            {
                                col++;
                            }
                            else
                            {
                                Error = new(lineNumber, colNumber + col, Error.Types.Syntax, string.Format("expected '}}', got {0}", expr[col]));
                                return (null, col);
                            }
                        }

                        node = typeNode;
                    }

                    else
                    {
                        (node, int endCol) = ParseSymbol(expr[startPos..], lineNumber, colNumber + startPos);
                        if (node == null) return (null, endCol);
                        col = startPos + endCol;
                    }
                }

                // Eat whitespace
                while (col < expr.Length && char.IsWhiteSpace(expr[col])) col++;

                if (node != null)
                {
                    node.LineNumber = lineNumber;
                    exprNode.Children.Add(node);
                }

                // Stop if: Reached the end of the line? Comment? Bracket? Met and ending character?
                if (col == expr.Length || col < expr.Length && (expr[col] == '(' || (endChar != null && endChar.Contains(expr[col])))) break;
                if (expr[col] == CommentChar) return (exprNode, expr.Length);
            }

            // Type casting?
            if (col < expr.Length && expr[col] == '(')
            {
                int end = GetMatchingClosingItem(expr[col..], '(', ')');
                if (end == -1)
                {
                    Error = new(lineNumber, colNumber + col, Error.Types.Syntax, string.Format("unterminated bracket '{0}'", expr[col]));
                    return (null, col);
                }

                string str = expr[(col + 1)..(col + end)];
                UnresolvedType type = new(str);
                exprNode.CastType = type;
                col += end + 1;
            }

            // Eat whitespace
            while (col < expr.Length && char.IsWhiteSpace(expr[col])) col++;

            // The input string should end with `endChar` or a comment
            if (col < expr.Length)
            {
                if (expr[col] == CommentChar)
                {

                }
                else if (endChar == null)
                {
                    Error = new(lineNumber, colNumber + col, Error.Types.Syntax, string.Format("expected end of line, got {0}", expr[col]));
                    return (null, col);
                }
                else if (endChar.Contains(expr[col]))
                {

                }
                else
                {
                    Error = new(lineNumber, colNumber + col, Error.Types.Syntax, string.Format("expected {0}, got {1}", string.Join(" or ", endChar.Select(a => a == null ? "end of line" : a.ToString())), expr[col]));
                    return (null, col);
                }
            }
            else
            {
                if (endChar != null && !endChar.Contains(null))
                {
                    Error = new(lineNumber, colNumber + col, Error.Types.Syntax, "unexpected end of line");
                    return (null, col);
                }
            }

            return (exprNode, col);
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

        public string GetErrorString()
        {
            return Error == null ? "" : ErrorToString(Error);
        }

        private static readonly Regex NonWhitespaceRegex = new("[^\\s]");

        /// <summary>
        /// Given an error, return the error as a string
        /// </summary>
        private string ErrorToString(Error error)
        {
            string str = error.ToString() + Environment.NewLine;

            string[] lines = Source.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            string line = lines[error.LineNumber];
            int origLength = line.Length;
            line = line.TrimStart();
            string lineNumberS = (error.LineNumber + 1).ToString();
            int colIdx = error.ColumnNumber - (origLength - line.Length);

            str += Environment.NewLine + (error.LineNumber + 1) + " | " + line;
            string pre = new(' ', lineNumberS.Length);
            string before = NonWhitespaceRegex.Replace(line[..colIdx], " ");
            string after = NonWhitespaceRegex.Replace(line[colIdx..], " ");
            str += Environment.NewLine + pre + "   " + before + "^" + after;

            return str;
        }
    }
}
