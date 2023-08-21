﻿using UglyLang.Source.Functions;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.AST
{
    /// <summary>
    /// A node which represents property access
    /// </summary>
    public class ChainedSymbolNode : AbstractSymbolNode
    {
        public readonly List<SymbolNode> Symbols = new();

        public override string GetSymbolString()
        {
            return string.Join('.', Symbols.Select(s => s.GetSymbolString()));
        }

        // Override position properties
        public new int LineNumber => Symbols[0].LineNumber;
        public new int ColumnNumber => Symbols[0].ColumnNumber;

        /// <summary>
        /// Get the values of this symbol chain. Return (value, valueProperty, valuesParent), or null if error (see context.Error).
        /// </summary>
        private (ISymbolValue, Property?, Value?)? RetrieveValues(Context context, ISymbolContainer container)
        {
            if (Symbols.Count == 0)
                throw new InvalidOperationException();

            ISymbolValue? parent = null;
            Property? parentProperty = null;
            Value? grandparent = null;

            foreach (SymbolNode symbol in Symbols)
            {
                if (parent == null)
                {
                    parent = symbol.Evaluate(context, container);
                    if (parent == null)
                        return null;
                }
                else
                {
                    if (parent is Value parentValue)
                    {
                        grandparent = parentValue;
                        if (parentValue.HasProperty(symbol.Symbol))
                        {
                            // Get the property
                            parentProperty = parentValue.GetProperty(symbol.Symbol);
                            parent = parentProperty.GetValue();

                            // if it is a function, call it
                            if (parent is ICallable func)
                            {
                                // Push new stack context
                                context.PushStackContext(symbol.LineNumber, symbol.ColumnNumber, StackContextType.Function, symbol.Symbol);

                                // Evaluate arguments
                                List<Value> arguments = new();
                                if (symbol.CallArguments != null)
                                {
                                    foreach (ExprNode expr in symbol.CallArguments)
                                    {
                                        Value? arg = expr.Evaluate(context, container);
                                        if (arg == null || context.Error != null)
                                            return null;

                                        arguments.Add(arg);
                                    }
                                }

                                // Call function with given arguments
                                Signal signal = func.Call(context, arguments, symbol.LineNumber, symbol.ColumnNumber);
                                if (signal == Signal.ERROR)
                                {
                                    if (context.Error != null)
                                    {
                                        context.Error.LineNumber = symbol.LineNumber;
                                        context.Error.ColumnNumber = symbol.ColumnNumber + symbol.Symbol.Length;
                                    }

                                    return null;
                                }

                                // Fetch return value
                                parent = context.GetFunctionReturnValue() ?? new EmptyValue();

                                // Pop stack context
                                context.PopStackContext();
                            }
                            else if (parent is Value pValue)
                            {
                                if (symbol.CallArguments != null && symbol.CallArguments.Count != 0)
                                {
                                    context.Error = new(symbol.LineNumber, symbol.ColumnNumber, Error.Types.Syntax, string.Format("value of type {0} is not callable", pValue.Type));
                                    return null;
                                }
                            }
                            else
                            {
                                throw new InvalidOperationException();
                            }
                        }
                        else
                        {
                            context.Error = new(symbol.LineNumber, symbol.ColumnNumber, Error.Types.Type, string.Format("cannot get property {0} of type {1}", symbol.Symbol, parentValue.Type));
                            return null;
                        }
                    }
                    else
                    {
                        context.Error = new(symbol.LineNumber, symbol.ColumnNumber, Error.Types.Type, "cannot get property of non-value");
                        return null;
                    }
                }
            }

            if (parent == null)
            {
                throw new InvalidOperationException();
            }

            return new(parent, parentProperty, grandparent);
        }

        public override Value? Evaluate(Context context, ISymbolContainer container)
        {
            var values = RetrieveValues(context, container);
            if (values == null)
                return null; // Propagate

            (ISymbolValue value, Property? _, ISymbolValue? _) = values.Value;

            if (value is Value val)
                return val;

            throw new InvalidOperationException(); // Should not happen.
        }

        public override bool SetValue(Context context, ISymbolContainer container, Value value, bool forceCast = false)
        {
            var values = RetrieveValues(context, container);
            if (values == null)
                return false; // Propagate

            (ISymbolValue oldChild, Property? property, Value? parent) = values.Value;

            if (parent == null || property == null)
                throw new NullReferenceException();

            // Is readonly?
            if (property.IsReadonly)
            {
                SymbolNode symbol = Symbols[^1];
                context.Error = new(symbol.LineNumber, symbol.ColumnNumber, Error.Types.Name, string.Format("cannot set {0} as property {1} is read-only", GetSymbolString(), symbol.Symbol));
                return false;
            }

            // Make sure that the types line up
            if (property.GetValue() is Value && oldChild is Value oldValue)
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
                        bool isOk = parent.SetProperty(property.GetName(), value);
                        if (!isOk)
                        {
                            SymbolNode symbol = Symbols[^1];
                            context.Error = new(symbol.LineNumber, symbol.ColumnNumber, Error.Types.Name, string.Format("property {0} cannot be changed", symbol.Symbol));
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

        public override bool CastValue(Context context, ISymbolContainer container, Types.Type type)
        {
            var values = RetrieveValues(context, container);
            if (values == null)
                return false; // Propagate

            (ISymbolValue child, Property? property, Value? parent) = values.Value;

            if (parent == null || property == null)
                throw new NullReferenceException();

            // Is readonly?
            if (property.IsReadonly)
            {
                SymbolNode symbol = Symbols[^1];
                context.Error = new(symbol.LineNumber, symbol.ColumnNumber, Error.Types.Name, string.Format("cannot cast {0} to {1} as property {2} is read-only", GetSymbolString(), type, symbol.Symbol));
                return false;
            }

            // If the parent has rigid property types, DO NOT allow casting, even if the types are equal
            if (parent.Type.HasRigidPropertyTypes())
            {
                SymbolNode symbol = Symbols[^1];
                context.Error = new(symbol.LineNumber, symbol.ColumnNumber, Error.Types.Type, string.Format("cannot cast {0} to {1} as properties of type {2} are rigid, so their types cannot change", GetSymbolString(), type, parent.Type));
                return false;
            }

            if (child is Value oldValue)
            {
                // Cast the old value
                Value? newValue = oldValue.To(type);
                if (newValue == null)
                {
                    context.Error = new(LineNumber, ColumnNumber, Error.Types.Cast, string.Format("casting {0} of type {1} to type {2}", GetSymbolString(), oldValue.Type, type));
                    return false;
                }
                else
                {
                    bool isOk = parent.SetProperty(property.GetName(), newValue);
                    if (!isOk)
                    {
                        SymbolNode symbol = Symbols[^1];
                        context.Error = new(symbol.LineNumber, symbol.ColumnNumber, Error.Types.Name, string.Format("property {0} cannot be cast", symbol.Symbol));
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

        public override bool UpdateValue(Context context, ISymbolContainer container, Func<Context, Value, Value?> transformer, bool forceCast = false)
        {
            var values = RetrieveValues(context, container);
            if (values == null)
                return false; // Propagate

            (ISymbolValue oldChild, Property? property, Value? parent) = values.Value;

            if (parent == null || property == null)
                throw new NullReferenceException();

            // Is readonly?
            if (property.IsReadonly)
            {
                SymbolNode symbol = Symbols[^1];
                context.Error = new(symbol.LineNumber, symbol.ColumnNumber, Error.Types.Name, string.Format("cannot set {0} as property {1} is read-only", GetSymbolString(), symbol.Symbol));
                return false;
            }

            // Make sure that the types line up
            if (property.GetValue() is Value && oldChild is Value oldValue)
            {
                // Transform value
                Value? value = transformer(context, oldValue);
                if (value == null) return false;

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
                        bool isOk = parent.SetProperty(property.GetName(), value);
                        if (!isOk)
                        {
                            SymbolNode symbol = Symbols[^1];
                            context.Error = new(symbol.LineNumber, symbol.ColumnNumber, Error.Types.Name, string.Format("property {0} cannot be changed", symbol.Symbol));
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
                context.Error = new(LineNumber, ColumnNumber, Error.Types.Type, string.Format("cannot set {0}", GetSymbolString()));
                return false;
            }
        }
    }
}
