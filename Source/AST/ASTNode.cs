using UglyLang.Source.Values;

namespace UglyLang.Source.AST
{
    /// <summary>
    ///  Base class for a node in an AST
    /// </summary>
    public abstract class ASTNode
    {
        public int LineNumber = 0;
        public int ColumnNumber = 0;

        /// Retrieve a Value from a node
        public abstract Value? Evaluate(Context context);

        /// Execute a node
        public virtual Signal Action(Context context)
        {
            return Signal.NONE;
        }
    }
}
