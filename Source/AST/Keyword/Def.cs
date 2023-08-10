using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source;
using UglyLang.Source.Functions;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.AST.Keyword
{
    /// <summary>
    /// Define a function
    /// </summary>
    public class DefKeywordNode : KeywordNode
    {
        public readonly string Name;
        public readonly List<(string, UnresolvedType)> Arguments;
        public readonly UnresolvedType ReturnType; // If NULL, returns nothing
        public ASTStructure? Body;

        public DefKeywordNode(string name, List<(string, UnresolvedType)> arguments, UnresolvedType returnType)
        {
            Name = name;
            Arguments = arguments;
            Body = null;
            ReturnType = returnType;
        }

        public override Signal Action(Context context)
        {
            if (Body == null) throw new NullReferenceException();

            // Check if the function is already defined
            UserFunction func;
            if (context.HasVariable(Name))
            {
                ISymbolValue variable = context.GetVariable(Name);
                if (variable is Function funcValue)
                {
                    if (funcValue is UserFunction userFunc)
                    {
                        func = userFunc;
                    }
                    else
                    {
                        context.Error = new(LineNumber, ColumnNumber, Error.Types.Name, string.Format("built-in function {0} cannot be overloaded", Name));
                        return Signal.ERROR;
                    }
                }
                else
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Name, string.Format("'{0}' is already defined and is not a function", Name));
                    return Signal.ERROR;
                }
            }
            else
            {
                func = new UserFunction(ReturnType);
                context.CreateVariable(Name, func);
            }

            // Add overload
            func.AddOverload(Arguments, Body);

            return Signal.NONE;
        }
    }
}
