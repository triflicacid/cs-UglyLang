using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UglyLang.source
{
    public partial class Parser
    {
        public Error? Error = null;
        public AST? AST = null;
        public Parser()
        {

        }

        public void Parse(string program)
        {
            AST = new();
            //Error = new(0, 0, Error.Types.General, "Parse method not implemented");
            Error = null;

            string[] lines = program.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            for (int lineNumber = 0, colNumber = 0; lineNumber < lines.Length; lineNumber++, colNumber = 0)
            {
                string line = lines[lineNumber];

                // Extract keyword
                string keyword = "";
            
                while (colNumber < line.Length && char.IsLetter(line[colNumber]))
                {
                    keyword += line[colNumber++];
                }

                // Is the line empty?
                if (colNumber == line.Length)
                {
                    continue;
                }

                // Is a comment?
                if (line[colNumber] == ';')
                {
                    continue;
                }

                //Console.WriteLine(string.Format("Line {0}: keyword {1}", lineNumber, keyword));

                // Check if the keyword exists
                if (keyword.Length == 0)
                {
                    Error = new(lineNumber, colNumber, Error.Types.Syntax, string.Format("expected keyword, got '{0}'", line[colNumber]));
                    AST = null;
                    break;
                }
                else if (!KeywordNode.KeywordDict.ContainsKey(keyword))
                {
                    Error = new(lineNumber, colNumber, Error.Types.Syntax, string.Format("unknown keyword '{0}'", keyword));
                    AST = null;
                    break;
                }

                var keywordInfo = KeywordNode.KeywordDict[keyword];
                string before = "", after = "";

                if (keywordInfo != null)
                {
                    if (keywordInfo.Before)
                    {
                        // Eat whitespace
                        while (colNumber < line.Length && line[colNumber] == ' ') colNumber++;

                        // Extract symbol
                        int beforeColNumber = colNumber;
                        while (colNumber < line.Length && char.IsLetterOrDigit(line[colNumber])) before += line[colNumber++];

                        if (!IsValidSymbol(before))
                        {
                            Error = new(lineNumber, beforeColNumber, Error.Types.Syntax, "invalid symbol");
                            AST = null;
                            break;
                        }
                    }

                    if (keywordInfo.After)
                    {
                        // Eat whitespace
                        while (colNumber < line.Length && line[colNumber] == ' ') colNumber++;

                        // Colon?
                        if (colNumber >= line.Length)
                        {
                            Error = new(lineNumber, colNumber, Error.Types.Syntax, "expected colon ':', got end of line");
                            AST = null;
                        }
                        else if (line[colNumber] == ':')
                        {
                            colNumber++;
                            
                            // Eat whitespace
                            while (colNumber < line.Length && line[colNumber] == ' ') colNumber++;

                            after = line[colNumber..];
                            colNumber = line.Length;
                        }
                        else
                        {
                            Error = new(lineNumber, colNumber, Error.Types.Syntax, string.Format("expected colon ':', got '{0}'", line[colNumber]));
                            AST = null;
                            break;
                        }
                    }
                }

                // Must be at end of line
                if (colNumber < line.Length)
                {
                    Error = new(lineNumber, colNumber, Error.Types.Syntax, string.Format("expected end of line, got '{0}'", line[colNumber]));
                    AST = null;
                    break;
                }

                // Create keyword node
                KeywordNode keywordNode;
                switch (keyword)
                {
                    case "LET":
                        {
                            ExprNode? expr = ParseExpression(after, lineNumber, colNumber);
                            if (expr == null) return; // Propagate error

                            keywordNode = new LetKeywordNode(before, expr);
                            break;
                        }
                    case "PRINT":
                        {
                            ExprNode? expr = ParseExpression(after, lineNumber, colNumber);
                            if (expr == null) return; // Propagate error

                            keywordNode = new PrintKeywordNode(expr);
                            break;
                        }
                    default:
                        throw new Exception("Reached default statement in switch, which should not happen.");
                }

                AST.AddNode(keywordNode);
            }
        }

        /// <summary>
        /// Return whether or not the provided string is a valid symbol
        /// </summary>
        public static bool IsValidSymbol(string symbol)
        {
            return SymbolRegex().IsMatch(symbol);
        }

        [GeneratedRegex("^[A-Za-z_\\$][A-Za-z_\\$0-9]*$")]
        private static partial Regex SymbolRegex();

        [GeneratedRegex("[A-Za-z_\\$]")]
        private static partial Regex LeadingSymbolCharRegex();

        [GeneratedRegex("^(?<symbol>[A-Za-z_\\$][A-Za-z_\\$0-9]*)")]
        private static partial Regex LeadingSymbolRegex();

        /// <summary>
        /// Extract the leading symbol from the given string
        /// </summary>
        private string ExtractSymbolFromString(string str)
        {
            Match match = LeadingSymbolRegex().Match(str);
            return match.Groups["symbol"].Value;
        }

        /// <summary>
        /// Parse a string as an expresion
        /// </summary>
        private ExprNode? ParseExpression(string expr, int lineNumber = 0, int colNumber = 0)
        {
            var node = _ParseExpression(expr, lineNumber, colNumber);
            return node == null ? null : new ExprNode(node);
        }

        private ASTNode? _ParseExpression(string expr, int lineNumber, int colNumber)
        {
            int col = 0;

            // Eat whitespace
            while (col < expr.Length && expr[col] == ' ') col++;

            // Is end of the line?
            if (col == expr.Length)
            {
                Error = new(lineNumber, colNumber + col, Error.Types.Syntax, "unexpected end of line");
                return null;
            }

            // Is a string literal?
            if (expr[col] == '"')
            {
                (string str, int end) = ExtractString(expr[col..]);

                if (end == -1)
                {
                    Error = new(lineNumber, colNumber + col, Error.Types.Syntax, "unterminated string literal");
                    return null;
                }

                col += end + 1;
                while (col < expr.Length && expr[col] == ' ') col++;

                if (col < expr.Length)
                {
                    Error = new(lineNumber, colNumber + col, Error.Types.Syntax, string.Format("expected end of line, got {0}", expr[col]));
                    return null;
                }

                return new ValueNode(new StringValue(str)) { LineNumber = lineNumber, ColumnNumber = colNumber + col };
            }

            // Is a symbol?
            else if (LeadingSymbolCharRegex().IsMatch(expr[col].ToString()))
            {
                string symbol = ExtractSymbolFromString(expr[col..]);
                col += symbol.Length;
                // TODO: call with parameters?

                while (col < expr.Length && expr[col] == ' ') col++;
                if (col < expr.Length)
                {
                    Error = new(lineNumber, colNumber + col, Error.Types.Syntax, string.Format("expected end of line, got {0}", expr[col]));
                    return null;
                }

                return new SymbolNode(symbol) { LineNumber = lineNumber, ColumnNumber = colNumber + col };
            }

            else
            {
                Error = new(lineNumber, colNumber + col, Error.Types.Syntax, string.Format("invalid syntax: '{0}'", expr[col]));
                return null;
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
    }
}
