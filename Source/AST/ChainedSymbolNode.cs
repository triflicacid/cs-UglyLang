using UglyLang.Source.Types;
using UglyLang.Source.Values;
using static UglyLang.Source.Functions.Function;

namespace UglyLang.Source.AST
{
    /// <summary>
    /// A node which represents property access
    /// </summary>
    public class ChainedSymbolNode : AbstractSymbolNode
    {
        public readonly List<ASTNode> Components = new();

        public override string GetSymbolString()
        {
            return string.Join('.', Components.Select(s => s is AbstractSymbolNode asn ? asn.GetSymbolString() : "[..]"));
        }

        // Override position properties
        public new int LineNumber => Components[0].LineNumber;
        public new int ColumnNumber => Components[0].ColumnNumber;

        /// <summary>
        /// Get the values of this symbol chain. Return (value, valueVariable, valuesParent), or null if error (see context.Error).
        /// </summary>
        private (ISymbolValue, Variable?, Value?)? RetrieveValues(Context context)
        {
            if (Components.Count == 0)
                throw new InvalidOperationException();

            ISymbolValue? parent = null;
            Variable? parentVar = null;
            Value? grandparent = null;

            foreach (ASTNode component in Components)
            {
                if (parent == null)
                {
                    parent = component.Evaluate(context);
                    if (parent == null)
                        return null;
                }
                else
                {
                    string propertyName;
                    if (component is SymbolNode symbolNode)
                    {
                        propertyName = symbolNode.Symbol;
                    }
                    else
                    {
                        Value? rawValue = component.Evaluate(context);
                        if (rawValue == null)
                            return null;
                        if (rawValue.Type is not PrimitiveType)
                        {
                            context.Error = new(component.LineNumber, component.ColumnNumber, Error.Types.Type, string.Format("invalid propery type {0}", rawValue.Type));
                            return null;
                        }

                        Value? strValue = rawValue.To(Types.Type.StringT);
                        if (strValue == null)
                        {
                            context.Error = new(component.LineNumber, component.ColumnNumber, Error.Types.Cast, string.Format("casting {0} to {1}", rawValue.Type, "STRING"));
                            return null;
                        }

                        propertyName = ((StringValue)strValue).Value;
                    }

                    if (parent is Value parentValue)
                    {
                        grandparent = parentValue;
                        if (parentValue.HasProperty(propertyName))
                        {
                            // Get the property
                            parentVar = parentValue.GetProperty(propertyName);
                            parent = (ISymbolValue)parentVar.GetValue();
                            if (parent is Types.Type t)
                                parent = new TypeValue(t);

                            // if it is a function, call it
                            if (parent is ICallable func)
                            {
                                // Push new stack context
                                if (func is Method method)
                                {
                                    context.PushMethodStackContext(component.LineNumber, component.ColumnNumber, parentVar, propertyName, method.Owner);
                                }
                                else
                                {
                                    context.PushStackContext(component.LineNumber, component.ColumnNumber, StackContextType.Function, parentVar, propertyName);
                                }

                                // Evaluate arguments
                                List<Value> arguments = new();
                                if (component is SymbolNode symbol && symbol.CallArguments != null)
                                {
                                    foreach (ExprNode expr in symbol.CallArguments)
                                    {
                                        Value? arg = expr.Evaluate(context);
                                        if (arg == null || context.Error != null)
                                            return null;

                                        arguments.Add(arg);
                                    }
                                }

                                // Call function with given arguments
                                Signal signal = func.Call(context, arguments, component.LineNumber, component.ColumnNumber);
                                if (signal == Signal.ERROR)
                                    return null;

                                // Fetch return value
                                parent = context.GetFunctionReturnValue() ?? new EmptyValue();

                                // Pop stack context
                                context.PopStackContext();
                            }
                            else if (parent is Value pValue)
                            {
                                if (component is SymbolNode symbol)
                                {
                                    if (pValue is TypeValue typeValue && symbol.CallArguments != null) // Construct the type
                                    {
                                        Types.Type type = typeValue.Value.ResolveParametersAgainst(context.GetBoundTypeParams());
                                        if (type.IsParameterised())
                                        {
                                            context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("parameterised type {0} cannot be resolved", typeValue.Value));
                                            return null;
                                        }

                                        if (!type.CanConstruct())
                                        {
                                            context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("type {0} cannot be constructed", type));
                                            return null;
                                        }

                                        // Evaluate arguments
                                        List<Value> arguments = new();
                                        foreach (ExprNode expr in symbol.CallArguments)
                                        {
                                            Value? arg = expr.Evaluate(context);
                                            if (arg == null)
                                                return null;
                                            if (context.Error != null)
                                            {
                                                return null; // Propagate error
                                            }

                                            arguments.Add(arg);
                                        }

                                        // Construct the type
                                        (Signal s, Value? newValue) = type.Construct(context, arguments, LineNumber, ColumnNumber);
                                        if (s == Signal.ERROR || newValue == null)
                                            return null;

                                        parent = newValue;
                                    }
                                    else if (symbol.CallArguments != null && symbol.CallArguments.Count != 0)
                                    {
                                        context.Error = new(component.LineNumber, component.ColumnNumber, Error.Types.Syntax, string.Format("value of type {0} is not callable", pValue.Type));
                                        return null;
                                    }
                                }
                            }
                            else
                            {
                                throw new InvalidOperationException(parent.ToString());
                            }
                        }
                        else
                        {
                            context.Error = new(component.LineNumber, component.ColumnNumber, Error.Types.Type, string.Format("cannot get property {0} of type {1}", propertyName, parentValue.Type));
                            return null;
                        }
                    }
                    else
                    {
                        context.Error = new(component.LineNumber, component.ColumnNumber, Error.Types.Type, "cannot get property of non-value");
                        return null;
                    }
                }
            }

            if (parent == null)
            {
                throw new InvalidOperationException();
            }

            return new(parent, parentVar, grandparent);
        }

        public override Value? Evaluate(Context context)
        {
            var values = RetrieveValues(context);
            if (values == null)
                return null; // Propagate

            (ISymbolValue value, Variable? _, ISymbolValue? _) = values.Value;

            if (value is Value val)
                return val;
            if (value is Types.Type t)
                return new TypeValue(t);

            throw new InvalidOperationException(value.ToString()); // Should not happen.
        }

        public override bool SetValue(Context context, Value value, bool forceCast = false)
        {
            var values = RetrieveValues(context);
            if (values == null)
                return false; // Propagate

            (ISymbolValue oldChild, var variable, Value? parent) = values.Value;

            if (parent == null || variable == null)
                throw new NullReferenceException();

            // Is readonly?
            if (variable.IsReadonly)
            {
                ASTNode latest = Components[^1];
                context.Error = new(latest.LineNumber, latest.ColumnNumber, Error.Types.Name, string.Format("cannot set {0} as property {1} is read-only", GetSymbolString(), variable.GetName()))
                {
                    AppendString = string.Format("Symbol {0} was defined at {1}:{2}", GetSymbolString(), variable.GetLineNumber() + 1, variable.GetColumnNumber() + 1),
                    AdditionalSource = ((ILocatable)variable).GetLocation()
                };
                return false;
            }

            // Make sure that the types line up
            if (variable.GetValue() is Value && oldChild is Value oldValue)
            {
                if (forceCast || oldValue.Type.DoesMatch(value.Type))
                {
                    Value? newValue = value.To(oldValue.Type);
                    if (newValue == null)
                    {
                        context.Error = new(LineNumber, ColumnNumber, Error.Types.Cast, string.Format("casting {0} to type {1}", GetSymbolString(), oldValue.Type));
                        return false;
                    }
                    else
                    {
                        // Update the property
                        bool isOk = parent.SetProperty(variable.GetName(), newValue);
                        if (!isOk)
                        {
                            ASTNode latest = Components[^1];
                            context.Error = new(latest.LineNumber, latest.ColumnNumber, Error.Types.Name, string.Format("property {0} cannot be changed", variable.GetName()));
                        }

                        return isOk;
                    }
                }
                else
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("cannot match {0} with {1} (in assignment to {2})", value.Type.ToString(), oldValue.Type.ToString(), GetSymbolString()));
                    return false;
                }
            }
            else
            {
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("cannot set {0} to type {1}", GetSymbolString(), value.Type));
                return false;
            }
        }

        public override bool CastValue(Context context, Types.Type type)
        {
            var values = RetrieveValues(context);
            if (values == null)
                return false; // Propagate

            (ISymbolValue child, var variable, Value? parent) = values.Value;

            if (parent == null || variable == null)
                throw new NullReferenceException();

            // Is readonly?
            if (variable.IsReadonly)
            {
                ASTNode latest = Components[^1];
                context.Error = new(latest.LineNumber, latest.ColumnNumber, Error.Types.Name, string.Format("cannot cast {0} to {1} as property {2} is read-only", GetSymbolString(), type, variable.GetName()))
                {
                    AppendString = string.Format("Symbol {0} was defined at {1}:{2}", GetSymbolString(), variable.GetLineNumber() + 1, variable.GetColumnNumber() + 1),
                    AdditionalSource = ((ILocatable)variable).GetLocation()
                };
                return false;
            }

            // If the parent has rigid property types, DO NOT allow casting, even if the types are equal
            if (parent.Type.HasRigidPropertyTypes())
            {
                ASTNode symbol = Components[^1];
                context.Error = new(symbol.LineNumber, symbol.ColumnNumber, Error.Types.Type, string.Format("cannot cast {0} to {1} as properties of type {2} are rigid, so their types cannot change", GetSymbolString(), type, parent.Type));
                return false;
            }

            if (child is Value oldValue)
            {
                // Cast the old value
                Value? newValue = oldValue.To(type);
                if (newValue == null)
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Cast, string.Format("casting {0} of type {1} to type {2}", GetSymbolString(), oldValue.Type, type))
                    {
                        AppendString = string.Format("Symbol {0} was defined at {1}:{2}", GetSymbolString(), variable.GetLineNumber() + 1, variable.GetColumnNumber() + 1),
                        AdditionalSource = ((ILocatable)variable).GetLocation()
                    };
                    return false;
                }
                else
                {
                    bool isOk = parent.SetProperty(variable.GetName(), newValue);
                    if (!isOk)
                    {
                        ASTNode latest = Components[^1];
                        context.Error = new(latest.LineNumber, latest.ColumnNumber, Error.Types.Name, string.Format("property {0} cannot be cast", variable.GetName()));
                    }

                    return isOk;
                }
            }
            else
            {
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("cannot cast symbol '{0}'", GetSymbolString()));
                return false;
            }

            throw new NotImplementedException();
        }

        public override bool UpdateValue(Context context, Func<Context, Value, Value?> transformer, bool forceCast = false)
        {
            var values = RetrieveValues(context);
            if (values == null)
                return false; // Propagate

            (ISymbolValue oldChild, var variable, Value? parent) = values.Value;

            if (parent == null || variable == null)
                throw new NullReferenceException();

            // Is readonly?
            if (variable.IsReadonly)
            {
                ASTNode latest = Components[^1];
                context.Error = new(latest.LineNumber, latest.ColumnNumber, Error.Types.Name, string.Format("cannot set {0} as property {1} is read-only", GetSymbolString(), variable.GetName()))
                {
                    AppendString = string.Format("Symbol {0} was defined at {1}:{2}", GetSymbolString(), variable.GetLineNumber() + 1, variable.GetColumnNumber() + 1),
                    AdditionalSource = ((ILocatable)variable).GetLocation()
                };
                return false;
            }

            // Make sure that the types line up
            if (variable.GetValue() is Value && oldChild is Value oldValue)
            {
                // Transform value
                Value? value = transformer(context, oldValue);
                if (value == null)
                    return false;

                if (forceCast || oldValue.Type.DoesMatch(value.Type))
                {
                    Value? newValue = value.To(oldValue.Type);
                    if (newValue == null)
                    {
                        context.Error = new(LineNumber, ColumnNumber, Error.Types.Cast, string.Format("casting {0} to type {1}", GetSymbolString(), oldValue.Type));
                        return false;
                    }
                    else
                    {
                        // Update the property
                        variable.SetValue(value);
                        //bool isOk = parent.SetProperty(variable.GetName(), value);
                        //if (!isOk)
                        //{
                        //    ASTNode latest = Components[^1];
                        //    context.Error = new(latest.LineNumber, latest.ColumnNumber, Error.Types.Name, string.Format("property {0} cannot be changed", variable.GetName()));
                        //}

                        return true;
                    }
                }
                else
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("cannot match {0} with {1} (in assignment to {2})", value.Type.ToString(), oldValue.Type.ToString(), GetSymbolString()));
                    return false;
                }
            }
            else
            {
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("cannot set {0}", GetSymbolString()));
                return false;
            }
        }
    }
}
