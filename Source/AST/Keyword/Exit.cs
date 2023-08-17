﻿namespace UglyLang.Source.AST.Keyword
{
    /// <summary>
    /// Exit the current nested structure
    /// </summary>
    public class ExitKeywordNode : KeywordNode
    {
        public ExitKeywordNode()
        { }

        public override Signal Action(Context context, ISymbolContainer container)
        {
            return Signal.EXIT_LOOP;
        }
    }
}
