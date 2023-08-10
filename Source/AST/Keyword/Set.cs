using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.AST.Keyword
{
    /// <summary>
    /// Node for the LET keyword - create a new variable
    /// </summary>
    public class SetKeywordNode : KeywordNode
    {
        public readonly string Name;
        public readonly List<string> Components;
        public readonly ExprNode Expr;

        public SetKeywordNode(string name, ExprNode expr) : base("SET")
        {
            Name = name;
            Components = name.Split('.').ToList();
            Expr = expr;
        }

        public override Signal Action(Context context)
        {
            Value? evaldValue = Expr.Evaluate(context);
            Value? parentValue = null;
            Property? property = null;

            if (evaldValue == null) // Propagate error?
            {
                return Signal.ERROR;
            }

            // Make sure that the types line up
            if (context.HasVariable(Components[0]))
            {
                ISymbolValue oldSymbolValue = context.GetVariable(Components[0]);

                // Lookup all components
                for (int i = 1; i < Components.Count; i++)
                {
                    if (oldSymbolValue is Value varValue)
                    {
                        parentValue = varValue;

                        if (varValue.HasProperty(Components[i]))
                        {
                            property = varValue.GetProperty(Components[i]);
                            oldSymbolValue = property.GetValue();
                        }
                        else
                        {
                            context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("cannot get property {0} of type {1}", Components[i], varValue.Type));
                            return Signal.ERROR;
                        }
                    }
                    else
                    {
                        context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("cannot get property {0}", Components[i]));
                        return Signal.ERROR;
                    }
                }

                if (oldSymbolValue is Value oldValue)
                {
                    if (oldValue.Type.DoesMatch(evaldValue.Type))
                    {
                        Value? newValue = evaldValue.To(oldValue.Type);
                        if (newValue == null)
                        {
                            context.Error = new(LineNumber, ColumnNumber, Error.Types.Cast, string.Format("casting {0} to type {1}", Name, oldValue.Type));
                            return Signal.ERROR;
                        }
                        else
                        {
                            evaldValue = newValue;
                        }
                    }
                    else
                    {
                        context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("cannot match {0} with {1} (in assignment to {2})", evaldValue.Type.ToString(), oldValue.Type.ToString(), Name));
                        return Signal.ERROR;
                    }
                }
                else
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("cannot set symbol '{0}' to type {1}", Name, evaldValue.Type));
                    return Signal.ERROR;
                }
            }

            // Is a property or not?
            if (parentValue == null)
            {
                // Set like a normal variable
                context.SetVariable(Name, evaldValue);
            }
            else
            {
                if (property != null)
                {
                    if (property.IsReadonly)
                    {
                        context.Error = new(LineNumber, ColumnNumber, Error.Types.General, string.Format("property {0} of {1} is readonly (setting {2})", Components[^1], parentValue.Type, Name));
                        return Signal.ERROR;
                    }

                    bool isOk = parentValue.SetProperty(Components[^1], evaldValue);
                    if (!isOk)
                    {
                        context.Error = new(LineNumber, ColumnNumber, Error.Types.Name, string.Format("cannot set property {0} of {1} (setting {2} to {3})", Components[^1], parentValue.Type, Name, evaldValue.Type));
                        return Signal.ERROR;
                    }
                }
                else
                {
                    throw new InvalidOperationException(); // One of parentValue and property should not be null.
                }
            }

            return Signal.NONE;
        }
    }
}
