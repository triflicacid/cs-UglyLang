using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UglyLang.Source.Types
{
    /// <summary>
    /// A loose type wrapper representing an absence
    /// </summary>
    public class None : Type
    {
        public override bool Equals(Type other)
        {
            return other is None;
        }

        public override bool DoesMatch(Type other)
        {
            return false;
        }

        public override string ToString()
        {
            return "NONE";
        }
    }
}
