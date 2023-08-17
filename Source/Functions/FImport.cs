using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions
{
    /// <summary>
    /// Function to import and execute a file
    /// </summary>
    public class FImport : Function, IDefinedGlobally
    {
        public FImport()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "IMPORT";
        }



        internal class OverloadOne : FunctionOverload
        {
            private static readonly Types.Type[] Arguments = new Types.Type[] { new StringType() };

            public OverloadOne()
            : base(Arguments, new NamespaceType())
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                string filename = ((StringValue)arguments[0]).Value;
                (Signal signal, NamespaceValue? ns) = context.Import(filename, lineNo, colNo);
                if (ns != null) context.SetFunctionReturnValue(ns);
                return signal;
            }
        }
    }
}
