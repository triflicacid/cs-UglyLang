using UglyLang.Source.Types;
using UglyLang.Source.Values;
using Type = UglyLang.Source.Types.Type;

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
            Overloads.Add(new OverloadTwo());
        }

        public string GetDefinedName()
        {
            return "IMPORT";
        }



        internal class OverloadOne : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { new StringType() };

            public OverloadOne()
            : base(Arguments, new NamespaceType())
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                string filename = ((StringValue)arguments[0]).Value;
                (Signal signal, NamespaceValue? ns) = context.Import(filename, lineNo, colNo);
                if (ns != null)
                    context.SetFunctionReturnValue(ns);
                return signal;
            }
        }

        internal class OverloadTwo : FunctionOverload
        {
            private static readonly Type[] Arguments = new Type[] { new StringType(), new IntType() };

            public OverloadTwo()
            : base(Arguments, new NamespaceType())
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                string filename = ((StringValue)arguments[0]).Value;
                (Signal signal, NamespaceValue? ns) = context.Import(filename, lineNo, colNo, arguments[1].IsTruthy());
                if (ns != null)
                    context.SetFunctionReturnValue(ns);
                return signal;
            }
        }
    }
}
