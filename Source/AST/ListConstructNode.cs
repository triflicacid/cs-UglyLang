using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.AST
{
    public class ListConstructNode : ASTNode
    {
        public readonly UnresolvedType? ListType;
        public List<ExprNode> Members = new();

        public ListConstructNode()
        {
            ListType = null;
        }

        public ListConstructNode(UnresolvedType listType)
        {
            ListType = listType;
        }

        public override ListValue? Evaluate(Context context)
        {
            // Evaluate arguments
            List<Value> members = new();
            foreach (ExprNode expr in Members)
            {
                Value? arg = expr.Evaluate(context);
                if (arg == null)
                    return null;
                if (context.Error != null)
                    return null; // Propagate error

                members.Add(arg);
            }

            // Resolve type?
            ListType listType;
            if (ListType == null)
            {
                listType = new(members[0].Type);
            }
            else
            {
                Types.Type? rawType = ListType.Resolve(context);

                if (rawType == null)
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("failed to resolve '{0}' to a type", ListType.Value.GetSymbolString()));
                    return null;
                }

                Types.Type type = rawType.ResolveParametersAgainst(context.GetBoundTypeParams());
                if (type.IsParameterised())
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("parameterised type {0} cannot be resolved", rawType));
                    return null;
                }
                else if (!type.CanConstruct())
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("type {0} cannot be constructed", type));
                    return null;
                }
                else if (type is ListType lType)
                {
                    listType = lType;
                }
                else
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("cannot match {0} with a[] in list construct", type));
                    return null;
                }
            }

            // Do the types all line up?
            for (int i = 1; i < members.Count; i++)
            {
                if (!members[i].Type.Equals(listType.Member))
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("expected {0}, got {1}, in {2} construct (argument {3})", listType.Member, members[i].Type, listType, i + 1));
                    return null;
                }
            }

            // Build the list
            ListValue list = new(listType.Member);
            list.Value.AddRange(members);

            return list;
        }
    }
}
