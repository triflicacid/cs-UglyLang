using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

        public void Parse(string program)
        {
            AST = null;
            Error = null;

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
                else if (line[colNumber] == BlockCommentChar && colNumber + 1 < line.Length && line[colNumber + 1] == CommentChar)
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
            
                while (colNumber < line.Length && char.IsLetter(line[colNumber]))
                {
                    keyword += line[colNumber++];
                }

                //Console.WriteLine(string.Format("KEYWORD: {0}", keyword));

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
                    string functionName = line[beforeColNumber .. colNumber];

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

                    List<(string, UnresolvedType)> argumentPairs = new();
                    if (line[colNumber] == '>')
                    {
                        colNumber++;
                    }
                    else
                    {
                        // Expect "<name>: <type>"
                        while (true)
                        {
                            // Extract argument name
                            beforeColNumber = colNumber;
                            while (colNumber < line.Length && char.IsLetterOrDigit(line[colNumber]))
                                colNumber++;
                            string argName = line[beforeColNumber..colNumber];

                            if (!IsValidSymbol(argName))
                            {
                                Error = new(lineNumber, beforeColNumber, Error.Types.Syntax, string.Format("invalid symbol \"{0}\"", argName));
                                break;
                            }

                            // Check for duplicate names
                            foreach ((string iArgName, _) in argumentPairs)
                            {
                                if (iArgName == argName)
                                {
                                    Error = new(lineNumber, beforeColNumber, Error.Types.Name, string.Format("duplicate name '{0}' in argument list", argName));
                                    break;
                                }
                            }
                            if (Error != null)
                                break;

                            // Eat whitespace
                            while (colNumber < line.Length && char.IsWhiteSpace(line[colNumber]))
                                colNumber++;

                            // Expect colon
                            if (colNumber == line.Length || line[colNumber] != ':')
                            {
                                string got = colNumber == line.Length ? "end of line" : line[colNumber].ToString();
                                Error = new(lineNumber, colNumber, Error.Types.Syntax, string.Format("expected ':', got {0}", got));
                                break;
                            }
                            colNumber++;

                            // Eat whitespace
                            while (colNumber < line.Length && char.IsWhiteSpace(line[colNumber]))
                                colNumber++;

                            // Extract argument type
                            beforeColNumber = colNumber;
                            while (colNumber < line.Length && !(char.IsWhiteSpace(line[colNumber]) || line[colNumber] == '>' || line[colNumber] == ','))
                                colNumber++;

                            string argTypeString = line[beforeColNumber..colNumber];
                            UnresolvedType argType = new(argTypeString);

                            // Eat whitespace
                            while (colNumber < line.Length && char.IsWhiteSpace(line[colNumber]))
                                colNumber++;

                            // Expect '<' or comma
                            if (colNumber == line.Length || (line[colNumber] != '>' && line[colNumber] != ','))
                            {
                                string got = colNumber == line.Length ? "end of line" : line[colNumber].ToString();
                                Error = new(lineNumber, colNumber, Error.Types.Syntax, string.Format("expected ',' or '>', got {0}", got));
                                break;
                            }

                            // Add argument to list
                            argumentPairs.Add(new(argName, argType));

                            // Skip comma
                            if (line[colNumber] == ',')
                                colNumber++;

                            // Eat whitespace
                            while (colNumber < line.Length && char.IsWhiteSpace(line[colNumber]))
                                colNumber++;

                            if (colNumber == line.Length)
                            {
                                Error = new(lineNumber, colNumber, Error.Types.Syntax, "unexpected end of line");
                                break;
                            }

                            // End of argument list?
                            if (line[colNumber] == '>')
                                break;
                        }
                        if (Error != null)
                            break;
                    }

                    // Create keyword node and add to tree structure
                    DefKeywordNode node = new(functionName, argumentPairs, returnType);
                    trees.Peek().AddNode(node);

                    // Nest and add a new tree
                    trees.Push(new());

                    continue;
                }

                // Fetch keyword information - this will tell us what to parse.
                var keywordInfo = KeywordNode.KeywordDict[keyword];
                string before = "", after = "";

                if (keywordInfo != null)
                {
                    // Eat whitespace
                    while (colNumber < line.Length && char.IsWhiteSpace(line[colNumber])) colNumber++;

                    if (keywordInfo.Before == TriState.YES || keywordInfo.Before == TriState.OPTIONAL)
                    {
                        // Extract symbol
                        int beforeColNumber = colNumber;
                        while (colNumber < line.Length && !(char.IsWhiteSpace(line[colNumber]) || line[colNumber] == ':')) before += line[colNumber++];

                        if (keywordInfo.Before == TriState.YES)
                        {
                            if (before.Length == 0)
                            {
                                string got = colNumber == line.Length ? "end of line" : ":";
                                Error = new(lineNumber, beforeColNumber, Error.Types.Syntax, string.Format("expected symbol, got {0}", got));
                                break;
                            }
                            else if (!IsValidSymbol(before))
                            {
                                Error = new(lineNumber, beforeColNumber, Error.Types.Syntax, string.Format("invalid symbol \"{0}\"", before));
                                break;
                            }
                        }
                    }

                    if (keywordInfo.After == TriState.YES || keywordInfo.After == TriState.OPTIONAL)
                    {
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
                            
                            // Eat whitespace
                            while (colNumber < line.Length && char.IsWhiteSpace(line[colNumber])) colNumber++;

                            after = line[colNumber..];
                            colNumber = line.Length;

                            if (after.Length == 0)
                            {
                                Error = new(lineNumber, colNumber, Error.Types.Syntax, "expected expression, got end of line, after ':'");
                                break;
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
                            keywordNode = new CastKeywordNode(before, new UnresolvedType(after));
                            break;
                        }
                    case "DO":
                        {
                            (ExprNode? expr, _) = ParseExpression(after, lineNumber, colNumber);
                            if (expr == null) return; // Propagate error

                            keywordNode = new DoKeywordNode(expr);
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
                                Error = new(lineNumber, colNumber, Error.Types.Syntax, string.Format("mismatched {0}", keyword));
                            }
                            else
                            {
                                ASTStructure previousTree = trees.Pop();
                                ASTNode latest = trees.Peek().PeekNode();

                                if (latest is IfKeywordNode ifKeyword && ifKeyword.Conditions.Count > 0)
                                {
                                    (ExprNode? expr, _) = ParseExpression(after, lineNumber, colNumber);
                                    if (expr == null) return; // Propagate error

                                    ifKeyword.Conditions.Last().Body = previousTree;
                                    ifKeyword.Conditions.Add(new(expr));
                                }
                                else
                                {
                                    Error = new(lineNumber, colNumber, Error.Types.Syntax, string.Format("mismatched {0}", keyword));
                                }

                                createNewNest = true;
                            }

                            break;
                        }
                    case "END":
                        {
                            if (trees.Count < 2) // Nothing to close!
                            {
                                Error = new(lineNumber, colNumber, Error.Types.Syntax, string.Format("mismatched {0}", keyword));
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
                            ExprNode? expr = null;

                            if (after.Length != 0)
                            {
                                (expr, _) = ParseExpression(after, lineNumber, colNumber);
                                if (expr == null) return; // Propagate error
                            }

                            keywordNode = new ErrorKeywordNode(expr);
                            break;
                        }
                    case "EXIT":
                        {
                            if (trees.Count < 2)
                            {
                                Error = new(lineNumber, colNumber, Error.Types.Syntax, keyword);
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
                                Error = new(lineNumber, colNumber, Error.Types.Syntax, keyword);
                            }
                            else
                            {
                                ExprNode? expr = null;
                                if (after.Length > 0)
                                {
                                    (expr, _) = ParseExpression(after, lineNumber, colNumber);
                                    if (expr == null)
                                        return; // Propagate error
                                }

                                keywordNode = new FinishKeywordNode(expr);
                            }
                            break;
                        }
                    case "IF":
                        {
                            (ExprNode? expr, _) = ParseExpression(after, lineNumber, colNumber);
                            if (expr == null) return; // Propagate error

                            IfKeywordNode ifKeyword = new();
                            ifKeyword.Conditions.Add(new(expr));
                            keywordNode = ifKeyword;
                            createNewNest = true;
                            break;
                        }
                    case "INPUT":
                        {
                            keywordNode = new InputKeywordNode(before);
                            break;
                        }
                    case "LET":
                        {
                            (ExprNode? expr, _) = ParseExpression(after, lineNumber, colNumber);
                            if (expr == null) return; // Propagate error

                            keywordNode = new LetKeywordNode(before, expr);
                            break;
                        }
                    case "LOOP":
                        {
                            LoopKeywordNode loopKeyword = new();

                            // Is there a counter?
                            if (before.Length > 0)
                            {
                                loopKeyword.Counter = before;
                            }

                            // Is there a condition?
                            if (after.Length > 0)
                            {
                                (ExprNode? expr, _) = ParseExpression(after, lineNumber, colNumber);
                                if (expr == null) return; // Propagate error
                                loopKeyword.Condition = expr;
                            }

                            keywordNode = loopKeyword;
                            createNewNest = true;
                            break;
                        }
                    case "PRINT":
                        {
                            (ExprNode? expr, _) = ParseExpression(after, lineNumber, colNumber);
                            if (expr == null) return; // Propagate error

                            keywordNode = new PrintKeywordNode(expr, false);
                            break;
                        }
                    case "PRINTLN":
                        {
                            (ExprNode? expr, _) = ParseExpression(after, lineNumber, colNumber);
                            if (expr == null) return; // Propagate error

                            keywordNode = new PrintKeywordNode(expr, true);
                            break;
                        }
                    case "SET":
                        {
                            (ExprNode? expr, _) = ParseExpression(after, lineNumber, colNumber);
                            if (expr == null) return; // Propagate error

                            keywordNode = new SetKeywordNode(before, expr);
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
        /// Return whether or not the provided string is a valid symbol
        /// </summary>
        public static bool IsValidSymbol(string symbol)
        {
            return SymbolRegex.IsMatch(symbol);
        }

        private static readonly Regex SymbolRegex = new("^[A-Za-z_\\$][A-Za-z_\\$0-9\\.]*$");
        private static readonly Regex LeadingSymbolCharRegex = new("[A-Za-z_\\$]");
        private static readonly Regex LeadingSymbolRegex = new("^(?<symbol>[A-Za-z_\\$][A-Za-z_\\$0-9\\.]*)");
        private static readonly Regex NumberRegex = new("^(?<number>-?(0|[1-9]\\d*)(\\.\\d+)?)");
        private static readonly Regex StopParsingCharRegex = new("[\\s<>{}]");

        /// <summary>
        /// Extract the leading symbol from the given string
        /// </summary>
        private static string ExtractSymbolFromString(string str)
        {
            Match match = LeadingSymbolRegex.Match(str);
            return match.Groups["symbol"].Value;
        }

        /// <summary>
        /// Extract a number from a string
        /// </summary>
        private static string ExtractNumberFromString(string str) {
            Match match = NumberRegex.Match(str);
            return match.Groups["number"].Value;
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

                ASTNode node;
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

                    col += end + 1;
                    node = new ValueNode(new StringValue(str));
                }

                // Is a number?
                else if (char.IsDigit(expr[col]) || (expr[col] == '-' && col + 1 < expr.Length && char.IsDigit(expr[col + 1])))
                {
                    string str = ExtractNumberFromString(expr[col..]);
                    col += str.Length;
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
                    while (col < expr.Length && !StopParsingCharRegex.IsMatch(expr[col].ToString()))
                        col++;

                    string s = expr[startPos..col];
                    node = new TypeNode(new(s));
                }

                // Extract the next word and proceed from there
                else
                {
                    // Extract symbol, then extract all until whitespace
                    string symbol = ExtractSymbolFromString(expr[col..]);
                    startPos = col;
                    while (col < expr.Length && !StopParsingCharRegex.IsMatch(expr[col].ToString())) col++;
                    string str = expr[startPos..col];
                    bool isTypeless = str.Length == 0; // If typeless, assume list

                    // Eat whitespace
                    while (col < expr.Length && char.IsWhiteSpace(expr[col])) col++;

                    // If there is a brace, it is a type constructor, else it is a symbol
                    if (col < expr.Length && expr[col] == '{')
                    {
                        TypeConstructNode typeNode = new(isTypeless ? null : new(str));

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
                                Error = new(lineNumber, colNumber + col, Error.Types.Syntax, string.Format("expected '}', got {0}", expr[col]));
                                return (null, col);
                            }
                        }

                        node = typeNode;
                    }

                    // Is it a symbol?
                    else if (IsValidSymbol(symbol))
                    {
                        col = startPos + symbol.Length;
                        SymbolNode symbolNode = new(symbol);

                        // Were any arguments provided?
                        if (col < expr.Length && expr[col] == '<')
                        {
                            int argStartPos = col;
                            symbolNode.CallArguments = new();
                            col++;

                            // Extract each argument, seperated by ','
                            while (col < expr.Length)
                            {
                                (ExprNode? argExpr, int end) = ParseExpression(expr[col..], lineNumber, colNumber + col, new char?[] { ',', '>', null });

                                if (argExpr == null)
                                {
                                    return (null, col);
                                }
                                else
                                {
                                    col += end;
                                    symbolNode.CallArguments.Add(argExpr);
                                    if (expr[col] == '>') break;
                                    col++;
                                }
                            }

                            if (col == expr.Length)
                            {
                                Error = new(lineNumber, colNumber + col, Error.Types.Syntax, "expected '>', got end of line");
                                return (null, col);
                            }
                            else if (expr[col] == '>')
                            {
                                col++;
                            }
                            else
                            {
                                Error = new(lineNumber, colNumber + col, Error.Types.Syntax, string.Format("expected '>', got {0}", expr[col]));
                                return (null, col);
                            }
                        }

                        node = symbolNode;
                    }

                    else
                    {
                        Error = new(lineNumber, colNumber + startPos, Error.Types.Syntax, string.Format("invalid syntax: '{0}'", expr[startPos]));
                        return (null, col);
                    }
                }

                // Eat whitespace
                while (col < expr.Length && char.IsWhiteSpace(expr[col])) col++;

                node.LineNumber = lineNumber;
                node.ColumnNumber = colNumber + startPos;
                exprNode.Children.Add(node);

                // Stop if: Reached the end of the line? Comment? Bracket? Met and ending character?
                if (col == expr.Length || col < expr.Length && (expr[col] == '(' || expr[col] == CommentChar || (endChar != null && endChar.Contains(expr[col])))) break;
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
    }
}
