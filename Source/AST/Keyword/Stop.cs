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
    /// Halt the program's execution
    /// </summary>
    public class StopKeywordNode : KeywordNode
    {
        public StopKeywordNode() : base("STOP")
        { }

        public override Signal Action(Context context)
        {
            return Signal.EXIT_PROG;
        }
    }
}
