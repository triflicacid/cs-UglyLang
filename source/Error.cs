using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UglyLang.source
{
    public class Error
    {
        public enum Types
        {
            General, // Generic error
            Syntax,  // The syntax is incorrect
            Name,    // Cannot find a name/variable
        }

        public readonly int LineNumber;
        public readonly int ColumnNumber;
        public readonly Types Type;
        public readonly string Message;

        public Error(int lineNumber, int colNumber, Types type, string message)
        {
            LineNumber = lineNumber;
            ColumnNumber = colNumber;
            Type = type;
            Message = message;
        }

        public override string ToString()
        {
            return Type.ToString() + " Error:" + LineNumber + ":" + ColumnNumber + " - " + Message;
        }
    }
}
