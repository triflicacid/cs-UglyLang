using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions
{
    /// <summary>
    /// Function to create a virtual file
    /// </summary>
    public class FCreateVirtualFile : Function, IDefinedGlobally
    {
        public FCreateVirtualFile()
        {
            Overloads.Add(new OverloadOne());
        }

        public string GetDefinedName()
        {
            return "CreateVirtualFile";
        }



        internal class OverloadOne : FunctionOverload
        {
            private static readonly Types.Type[] Arguments = new Types.Type[] { Types.Type.StringT, Types.Type.StringT };

            public OverloadOne()
            : base(Arguments, Types.Type.EmptyT)
            { }

            public override Signal Call(Context context, List<Value> arguments, TypeParameterCollection typeParameters, int lineNo, int colNo)
            {
                string filename = ((StringValue)arguments[0]).Value;

                if (context.ParseOptions.FileSources.ContainsKey(filename))
                {
                    context.Error = new(lineNo, colNo, Error.Types.Argument, string.Format("cannot create virtual file as '{0}' already exists", filename));
                    return Signal.ERROR;
                }

                string contents = ((StringValue)arguments[1]).Value;
                context.ParseOptions.FileSources.Add(filename, new(contents));
                return Signal.NONE;
            }
        }
    }
}
