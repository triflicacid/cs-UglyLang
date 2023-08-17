namespace UglyLang.Source
{
    public class Error
    {
        public enum Types
        {
            Import,  // Error whilst importing/in an import
            General, // Generic error
            Syntax,  // The syntax is incorrect
            Name,    // Cannot find a name/variable
            Type,    // Incorrect/mismatching type(s)
            Cast,    // Error whilst casting
            Raised,  // Used when the ERROR keyword is invoked
            Argument, // Error in a functions argument (used inside functions)
        }

        public int LineNumber;
        public int ColumnNumber;
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
            return Type.ToString() + " Error (" + (LineNumber + 1) + ":" + (ColumnNumber + 1) + ") - " + Message;
        }
    }
}
