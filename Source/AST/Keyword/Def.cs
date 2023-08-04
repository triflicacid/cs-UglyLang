﻿using System;
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
        public readonly Values.ValueType? ReturnType; // If NULL, returns nothing
        public ASTStructure? Body;

        public DefKeywordNode(string name, List<(string, Values.ValueType)> arguments, Values.ValueType? returnType) : base("DEF")
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
                        // Check if the return types match
                        if ((userFunc.ReturnType == null && ReturnType == null) || (userFunc.ReturnType != null && ReturnType != null && Value.Match((Values.ValueType)userFunc.ReturnType, (Values.ValueType)ReturnType)))
                        {
                            // Has this overload been seen before?
                            bool match = false;
                            foreach (var typeArray in userFunc.ArgumentTypes)
                            {
                                if (typeArray.Length == Arguments.Count)
                                {
                                    match = true;
                                    for (int i = 0; i < typeArray.Length && match; i++)
                                    {
                                        match = Value.Match(Arguments[i].Item2, typeArray[i]);
                                    }

                                    if (!match)
                                        break;
                                }
                                if (match)
                                    break;
                            }

                            if (match)
                            {
                                context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("{0} overload <{1}> -> {2} already exists", Name, string.Join(",", Arguments.Select(p => p.Item2.ToString())), ReturnType.ToString()));
                                return Signal.ERROR;
                            }

                            func = userFunc;
                        }
                        else
                        {
                            context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("cannot match type {0} with {1}", ReturnType == null ? "(none)" : ReturnType, userFunc.ReturnType == null ? "(none)" : userFunc.ReturnType));
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
