using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UglyLang.Source.Values
{
    /// <summary>
    /// A value which contains nothing - undefined/null
    /// </summary>
    public class EmptyValue : Value
    {
        public EmptyValue()
        {
            Type = ValueType.EMPTY;
        }

        public static EmptyValue From(Value value)
        {
            throw new NotSupportedException();
        }

        public override Value To(ValueType type)
        {
            throw new NotSupportedException();
        }
    }
}
