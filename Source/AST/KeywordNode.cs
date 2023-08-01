using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UglyLang.Source.AST
{
    /// <summary>
    /// Node containing the concept of a keyword/action
    /// </summary>
    public abstract class KeywordNode : ASTNode
    {
        public readonly string Keyword;

        public KeywordNode(string kw)
        {
            Type = ASTNodeType.KEYWORD;
            Keyword = kw;
        }

        public class KeywordInfo
        {
            /// Does the keyword expect something immediatley after it?
            public readonly bool Before;

            /// Does the keyword expect a colon and something after it?
            public readonly bool After;

            public KeywordInfo(bool before, bool after)
            {
                After = after;
                Before = before;
            }
        }

        public static readonly Dictionary<string, KeywordInfo> KeywordDict = new() {
            { "LET", new(true, true) },
            { "PRINT", new(false, true) },
            { "SET", new(true, true) },
        };
    }
}
