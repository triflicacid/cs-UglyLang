using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source;
using UglyLang.Source.Functions;
using UglyLang.Source.Values;

namespace UglyLang.Source.AST.Keyword
{
    /// <summary>
    /// Define a function
    /// </summary>
    public class DefKeywordNode : KeywordNode
    {
        public readonly string Name;
        public readonly List<(string, Values.ValueType)> Arguments;
        public readonly Values.ValueType ReturnType;
        public ASTStructure? Body;

        public DefKeywordNode(string name, List<(string, Values.ValueType)> arguments, Values.ValueType returnType) : base("DEF")
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
                Value variable = context.GetVariable(Name);
                if (variable is FuncValue funcValue)
                {
                    if (funcValue.Func is UserFunction userFunc)
                    {
                        if (Value.Match(userFunc.ReturnType, ReturnType))
                        {
                            func = userFunc;
                        }
                        else
                        {
                            context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("cannot match type {0} with {1}", ReturnType, userFunc.ReturnType));
                            return Signal.ERROR;
                        }
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
                context.CreateVariable(Name, new FuncValue(func));
            }

            // Add overload
            func.AddOverload(Arguments, Body);

            return Signal.NONE;
        }
    }
}
