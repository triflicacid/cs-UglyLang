using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Values;

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

        public override Value Evaluate(Context context)
        {
            throw new NotImplementedException("Should not call Evaluate on this node");
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
            { "CAST", new(true, true) },
            { "INPUT", new(true, false) },
            { "LET", new(true, true) },
            { "PRINT", new(false, true) },
            { "PRINTLN", new(false, true) },
            { "SET", new(true, true) },
        };
    }
}
