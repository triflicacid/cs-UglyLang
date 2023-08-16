using UglyLang.Source.Values;

namespace UglyLang.Source.AST
{
    /// <summary>
    /// Node containing the concept of a keyword/action
    /// </summary>
    public abstract class KeywordNode : ASTNode
    {
        public override Value Evaluate(Context context)
        {
            throw new NotImplementedException();
        }

        public class KeywordInfo
        {
            /// Does the keyword expect something immediatley after it?
            public readonly TriState Before;
            public readonly ParseOptions.Before BeforeItem;

            /// Does the keyword expect a colon and something after it?
            public readonly TriState After;
            public readonly ParseOptions.After AfterItem;

            public KeywordInfo(TriState before, ParseOptions.Before beforeItem, TriState after, ParseOptions.After afterItem)
            {
                After = after;
                Before = before;
                BeforeItem = beforeItem;
                AfterItem = afterItem;
            }
        }

        public static readonly Dictionary<string, KeywordInfo> KeywordDict = new() {
            { "CAST", new(TriState.YES, ParseOptions.Before.CHAINED_SYMBOL, TriState.YES, ParseOptions.After.TYPE) },
            { "DEF", new(TriState.YES, ParseOptions.Before.SYMBOL, TriState.NO, ParseOptions.After.EXPR) },
            { "DO", new(TriState.NO, ParseOptions.Before.NONE, TriState.YES, ParseOptions.After.EXPR) },
            { "ELSE", new(TriState.NO, ParseOptions.Before.NONE, TriState.NO, ParseOptions.After.NONE) },
            { "ELSEIF", new(TriState.NO, ParseOptions.Before.NONE, TriState.YES, ParseOptions.After.EXPR) },
            { "END", new(TriState.NO, ParseOptions.Before.NONE, TriState.NO, ParseOptions.After.NONE) },
            { "ERROR", new(TriState.NO, ParseOptions.Before.NONE, TriState.OPTIONAL, ParseOptions.After.EXPR) },
            { "EXIT", new(TriState.NO, ParseOptions.Before.NONE, TriState.NO, ParseOptions.After.NONE) },
            { "FINISH", new(TriState.NO, ParseOptions.Before.NONE, TriState.OPTIONAL, ParseOptions.After.EXPR) },
            { "IF", new(TriState.NO, ParseOptions.Before.NONE, TriState.YES, ParseOptions.After.EXPR) },
            { "INPUT", new(TriState.YES, ParseOptions.Before.CHAINED_SYMBOL, TriState.NO, ParseOptions.After.EXPR) },
            { "LET", new(TriState.YES, ParseOptions.Before.SYMBOL, TriState.YES, ParseOptions.After.EXPR) },
            { "LOOP", new(TriState.OPTIONAL, ParseOptions.Before.SYMBOL, TriState.OPTIONAL, ParseOptions.After.EXPR) },
            { "PRINT", new(TriState.NO, ParseOptions.Before.NONE, TriState.YES, ParseOptions.After.EXPR) },
            { "PRINTLN", new(TriState.NO, ParseOptions.Before.NONE, TriState.YES, ParseOptions.After.EXPR) },
            { "SET", new(TriState.YES, ParseOptions.Before.CHAINED_SYMBOL, TriState.YES, ParseOptions.After.EXPR) },
            { "STOP", new(TriState.NO, ParseOptions.Before.NONE, TriState.NO, ParseOptions.After.NONE) },
        };
    }
}
