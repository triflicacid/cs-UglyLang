namespace UglyLang.Source.AST.Keyword
{
    public class KeywordInfo
    {
        public readonly string Keyword;

        /// Does the keyword expect something immediatley after it?
        public readonly TriState Before;
        public readonly ParseOptions.Before BeforeItem;

        /// Does the keyword expect a colon and something after it?
        public readonly TriState After;
        public readonly ParseOptions.After AfterItem;

        /// Which keywords are permitted under this keyword. If null, allow all keywords.
        public readonly string[]? AllowUnder;

        /// Which keywords are permitted to be above this? If null, allow all keywords.
        public readonly string[]? AllowAbove;

        public KeywordInfo(string keyword, TriState before, ParseOptions.Before beforeItem, TriState after, ParseOptions.After afterItem, string[]? allowUnder = null, string[]? allowAbove = null)
        {
            Keyword = keyword;
            After = after;
            Before = before;
            BeforeItem = beforeItem;
            AfterItem = afterItem;
            AllowUnder = allowUnder;
            AllowAbove = allowAbove;
        }


        public static readonly List<KeywordInfo> List = new() {
            new("CAST", TriState.YES, ParseOptions.Before.CHAINED_SYMBOL, TriState.YES, ParseOptions.After.TYPE),
            new("CONST", TriState.YES, ParseOptions.Before.SYMBOL, TriState.YES, ParseOptions.After.EXPR),
            new("DEC", TriState.YES, ParseOptions.Before.CHAINED_SYMBOL, TriState.NO, ParseOptions.After.NONE),
            new("DEF", TriState.YES, ParseOptions.Before.SYMBOL, TriState.NO, ParseOptions.After.EXPR),
            new("DO", TriState.NO, ParseOptions.Before.NONE, TriState.OPTIONAL, ParseOptions.After.EXPR),
            new("ELSE", TriState.NO, ParseOptions.Before.NONE, TriState.NO, ParseOptions.After.NONE),
            new("ELSEIF", TriState.NO, ParseOptions.Before.NONE, TriState.YES, ParseOptions.After.EXPR),
            new("END", TriState.NO, ParseOptions.Before.NONE, TriState.NO, ParseOptions.After.NONE),
            new("ERROR", TriState.NO, ParseOptions.Before.NONE, TriState.OPTIONAL, ParseOptions.After.EXPR),
            new("EXIT", TriState.NO, ParseOptions.Before.NONE, TriState.NO, ParseOptions.After.NONE),
            new("FIELD", TriState.YES, ParseOptions.Before.SYMBOL, TriState.YES, ParseOptions.After.TYPE, null, new string[] { "TYPE" }),
            new("FINISH", TriState.NO, ParseOptions.Before.NONE, TriState.OPTIONAL, ParseOptions.After.EXPR),
            new("IF", TriState.NO, ParseOptions.Before.NONE, TriState.YES, ParseOptions.After.EXPR),
            new("IMPORT", TriState.OPTIONAL, ParseOptions.Before.SYMBOL, TriState.YES, ParseOptions.After.STRING),
            new("INC", TriState.YES, ParseOptions.Before.CHAINED_SYMBOL, TriState.NO, ParseOptions.After.NONE),
            new("INPUT", TriState.YES, ParseOptions.Before.CHAINED_SYMBOL, TriState.NO, ParseOptions.After.EXPR),
            new("LET", TriState.YES, ParseOptions.Before.SYMBOL, TriState.YES, ParseOptions.After.EXPR),
            new("LOOP", TriState.OPTIONAL, ParseOptions.Before.SYMBOL, TriState.OPTIONAL, ParseOptions.After.EXPR),
            new("NAMESPACE", TriState.YES, ParseOptions.Before.SYMBOL, TriState.NO, ParseOptions.After.NONE, new string[] { "NAMESPACE", "CONST", "DEF", "LET", "TYPE", "END" }),
            new("NEW", TriState.NO, ParseOptions.Before.NONE, TriState.NO, ParseOptions.After.NONE, null, new string[] { "TYPE" }),
            new("PRINT", TriState.NO, ParseOptions.Before.NONE, TriState.YES, ParseOptions.After.EXPR),
            new("PRINTLN", TriState.NO, ParseOptions.Before.NONE, TriState.YES, ParseOptions.After.EXPR),
            new("SET", TriState.YES, ParseOptions.Before.CHAINED_SYMBOL, TriState.YES, ParseOptions.After.EXPR),
            new("STOP", TriState.NO, ParseOptions.Before.NONE, TriState.NO, ParseOptions.After.NONE),
            new("TYPE", TriState.YES, ParseOptions.Before.SYMBOL, TriState.NO, ParseOptions.After.NONE, new string[] { "CONST", "DEF", "LET", "NEW", "FIELD", "END" }),
        };

        public static readonly Dictionary<string, KeywordInfo> Lookup = new();

        static KeywordInfo()
        {
            foreach (KeywordInfo kw in List)
            {
                Lookup.Add(kw.Keyword, kw);
            }
        }
    }
}
