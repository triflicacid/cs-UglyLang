using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UglyLang.Source.Types
{
    public class TypeParameter : Type
    {
        public readonly string Symbol;

        public TypeParameter(string symbol)
        {
            Symbol = symbol;
        }

        public override bool Equals(Type other)
        {
            return other is TypeParameter;
        }

        public override bool DoesMatch(Type other)
        {
            throw new InvalidOperationException();
        }

        public override string ToString()
        {
            return Symbol;
        }
    }
}
