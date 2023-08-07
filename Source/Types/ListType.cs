using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UglyLang.Source.Types
{
    /// <summary>
    /// A collection of one type
    /// </summary>
    public class ListType : Type
    {
        public readonly Type Member;

        public ListType(Type member)
        {
            Member = member;
        }

        public override bool Equals(Type other)
        {
            return other is ListType list && Member.Equals(list.Member);
        }

        public override bool DoesMatch(Type other)
        {
            return other is ListType list && Member.DoesMatch(list.Member);
        }

        public override string ToString()
        {
            return Member.ToString() + "[]";
        }
    }
}
