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
    /// Exit the current nested structure
    /// </summary>
    public class ExitKeywordNode : KeywordNode
    {
        public ExitKeywordNode()
        { }

        public override Signal Action(Context context)
        {
            return Signal.EXIT_LOOP;
        }
    }
}
