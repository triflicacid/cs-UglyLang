namespace UglyLang.Source.AST
{
    public class ASTStructure
    {
        private readonly List<ASTNode> Nodes = new();

        public void AddNode(ASTNode node)
        {
            Nodes.Add(node);
        }

        public ASTNode PeekNode()
        {
            if (Nodes.Count == 0)
                throw new NullReferenceException();
            return Nodes[^1];
        }

        public Signal Evaluate(Context context)
        {
            foreach (ASTNode root in Nodes)
            {
                Signal signal = root.Action(context);
                if (signal != Signal.NONE)
                    return signal;
            }
            return Signal.NONE;
        }

        public IEnumerator<ASTNode> GetEnumerator()
        {
            return Nodes.GetEnumerator();
        }
    }
}
