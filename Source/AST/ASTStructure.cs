using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source;

namespace UglyLang.Source.AST
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
            return roots[roots.Count - 1];
        }

        public Signal Evaluate(Context context)
        {
            foreach (ASTNode root in roots)
            {
                Signal signal = root.Action(context);
                if (signal != Signal.NONE) return signal;
            }
            return Signal.NONE;
        }
    }
}
