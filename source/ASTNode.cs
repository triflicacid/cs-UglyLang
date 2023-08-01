using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace UglyLang.source
{
    /// <summary>
    ///  Base class for a node in an AST
    /// </summary>
    public abstract class ASTNode
    {
        public ASTNodeType Type;
        public int LineNumber = 0;
        public int ColumnNumber = -1;

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
        LEAF,
        KEYWORD,
        VALUE,
        SYMBOL,
        EXPR,
    }

    /// <summary>
    /// A leaf node - a placeholder/dud
    /// </summary>
    public class LeafNode : ASTNode
    {
        public LeafNode()
        {
            Type = ASTNodeType.LEAF;
        }

        public override Value Evaluate(Context context)
        {
            throw new Exception("Should not call Evaluate on this node");
        }
    }

    /// <summary>
    /// A node which contains an expression. This class is only a semantic wrapper for another node.
    /// </summary>
    public class ExprNode : ASTNode
    {
        public ASTNode Child;

        public ExprNode(ASTNode child)
        {
            Type = ASTNodeType.EXPR;
            Child = child;
        }

        public override Value Evaluate(Context context)
        {
            return Child.Evaluate(context);
        }
    }

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
        };
    }

    /// <summary>
    /// Node which contains a literal value
    /// </summary>
    public class ValueNode : ASTNode
    {
        public Value Value;

        public ValueNode(Value value)
        {
            Type = ASTNodeType.VALUE;
            Value = value;
        }

        public override Value Evaluate(Context context)
        {
            return Value;
        }
    }

    public class SymbolNode : ASTNode
    {
        public string Symbol;
        public List<ExprNode> CallArguments;

        public SymbolNode(string symbol)
        {
            Type = ASTNodeType.SYMBOL;
            Symbol = symbol;
            CallArguments = new();
        }

        public override Value Evaluate(Context context)
        {
            if (CallArguments.Count == 0)
            {
                if (context.HasVariable(Symbol))
                {
                    return context.GetVariable(Symbol);
                }
                else
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Name, Symbol);
                    return new EmptyValue();
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    /// Node for the LET keyword - create a new variable
    /// </summary>
    public class LetKeywordNode : KeywordNode
    {
        public readonly string Name;
        public readonly ExprNode Value;

        public LetKeywordNode(string name, ExprNode value) : base("LET")
        {
            Name = name;
            Value = value;
        }

        public override Value Evaluate(Context context)
        {
            throw new Exception("Should not call Evaluate on this node");
        }

        public override Signal Action(Context context)
        {
            if (context.HasVariable(Name))
            {
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Name, string.Format("LET: Cannot re-define a name \"{0}\" which already exists.", Name));
                return Signal.ERROR;
            }
            else
            {
                Value evaldValue = Value.Evaluate(context);
                if (context.Error != null) // Propagate error?
                {
                    return Signal.ERROR;
                }

                context.CreateVariable(Name, evaldValue);
                return Signal.NONE;
            }
        }
    }

    /// <summary>
    /// Node for the PRINT keyword - print a value to the screen
    /// </summary>
    public class PrintKeywordNode : KeywordNode
    {
        public readonly ExprNode Expr;

        public PrintKeywordNode(ExprNode expr) : base("PRINT")
        {
            Expr = expr;
        }

        public override Value Evaluate(Context context)
        {
            throw new Exception("Should not call Evaluate on this node");
        }

        public override Signal Action(Context context)
        {
            Value value = Expr.Evaluate(context);
            if (context.Error != null) // Propagate error?
                return Signal.ERROR;

            Console.WriteLine(StringValue.From(value).Value);
            return Signal.NONE;
        }
    }
}
