using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source;
using UglyLang.Source.Values;

namespace UglyLang.Source.AST.Keyword
{
    /// <summary>
    /// Execute the body continuously and indefintely
    /// </summary>
    public class LoopKeywordNode : KeywordNode
    {
        public ASTStructure? Body;

        public LoopKeywordNode() : base("LOOP")
        {
            Body = null;
        }

        public override Signal Action(Context context)
        {
            if (Body == null) throw new NullReferenceException(); // Should not be the case

            while (true)
            {
                Signal signal = Body.Evaluate(context);
                if (signal != Signal.NONE)
                {
                    if (signal == Signal.EXIT_LOOP) return Signal.NONE; // Signal has been processed.
                    return signal;
                }
            }
        }
    }
}
