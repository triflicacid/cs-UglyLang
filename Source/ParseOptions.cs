﻿namespace UglyLang.Source
{
    public class ParseOptions
    {
        /// <summary>
        /// Before a keyword: what should we expect to parse?
        /// </summary>
        public enum Before
        {
            NONE,
            SYMBOL, // SymbolNode
            CHAINED_SYMBOL, // SymbolNode or ChainedSymbolNode
        }

        /// <summary>
        /// After a keyword: what should we expect to parse?
        /// </summary>
        public enum After
        {
            NONE,
            EXPR, // ExprNode
            TYPE, // SymbolNode with .Symbol set to the type string
            STRING, // StringNode
        }
    }
}