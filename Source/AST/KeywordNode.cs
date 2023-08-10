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
            public readonly TriState Before;

            /// Does the keyword expect a colon and something after it?
            public readonly TriState After;

            public KeywordInfo(TriState before, TriState after)
            {
                After = after;
                Before = before;
            }
        }

        public static readonly Dictionary<string, KeywordInfo> KeywordDict = new() {
            { "CAST", new(TriState.YES, TriState.YES) },
            { "DEF", new(TriState.YES, TriState.NO) },
            { "DO", new(TriState.NO, TriState.YES) },
            { "ELSE", new(TriState.NO, TriState.NO) },
            { "ELSEIF", new(TriState.NO, TriState.YES) },
            { "END", new(TriState.NO, TriState.NO) },
            { "ERROR", new(TriState.NO, TriState.OPTIONAL) },
            { "EXIT", new(TriState.NO, TriState.NO) },
            { "FINISH", new(TriState.NO, TriState.OPTIONAL) },
            { "IF", new(TriState.NO, TriState.YES) },
            { "INPUT", new(TriState.YES, TriState.NO) },
            { "LET", new(TriState.YES, TriState.YES) },
            { "LOOP", new(TriState.OPTIONAL, TriState.OPTIONAL) },
            { "PRINT", new(TriState.NO, TriState.YES) },
            { "PRINTLN", new(TriState.NO, TriState.YES) },
            { "SET", new(TriState.YES, TriState.YES) },
            { "STOP", new(TriState.NO, TriState.NO) },
        };
    }
}
