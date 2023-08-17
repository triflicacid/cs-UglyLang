﻿namespace UglyLang.Source.AST
{
    public class ASTStructure
    {
        private readonly List<ASTNode> roots = new();

        public void AddNode(ASTNode node)
        {
            roots.Add(node);
        }

        public ASTNode PeekNode()
        {
            if (roots.Count == 0) throw new NullReferenceException();
            return roots[^1];
        }

        public Signal Evaluate(Context context)
        {
            return Evaluate(context, context);
        }

        public Signal Evaluate(Context context, ISymbolContainer container)
        {
            foreach (ASTNode root in roots)
            {
                Signal signal = root.Action(context, container);
                if (signal != Signal.NONE) return signal;
            }
            return Signal.NONE;
        }
    }
}
