using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Values;

namespace UglyLang.Source.AST
{
    public class TypeConstructNode : ASTNode
    {
        public readonly Types.Type Construct;
        public readonly List<ExprNode> Arguments;

        public TypeConstructNode(Types.Type construct)
        {
            Construct = construct;
            Arguments = new();
        }

        public override Value? Evaluate(Context context)
        {
            Value? value;

            if (Arguments.Count == 0)
            {
                value = Construct.ConstructNoArgs(context);
            }
            else
            {
                // Evaluate each argument
                List<Value> evaldArguments = new();
                foreach (ExprNode node in Arguments)
                {
                   value = node.Evaluate(context);
                    if (value == null) return null; // Propagate error
                    evaldArguments.Add(value);
                }

                value = Construct.ConstructWithArgs(context, evaldArguments);
            }

            if (value == null)
            {
                if (context.Error != null)
                {
                    context.Error.LineNumber = LineNumber;
                    context.Error.ColumnNumber = ColumnNumber;
                }

                return null;
            }
            else
            {
                return value;
            }
        }
    }
}
