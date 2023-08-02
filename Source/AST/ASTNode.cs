using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source;
using UglyLang.Source.Values;

namespace UglyLang.Source.AST
{
    /// <summary>
    ///  Base class for a node in an AST
    /// </summary>
    public abstract class ASTNode
    {
        public ASTNodeType Type;
        public int LineNumber = 0;
        public int ColumnNumber = 0;
        public Values.ValueType? CastType = null;

        /// Retrieve a Value from a node
        public abstract Value Evaluate(Context context);

        /// Execute a node
        public virtual Signal Action(Context context)
        {
            return Signal.NONE;
        }
    }

    /// <summary>
    /// Enumeration listing all the possible types of AST Node
    /// </summary>
    public enum ASTNodeType
    {
        KEYWORD,
        VALUE,
        SYMBOL,
        EXPR,
    }
}
