using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source
{
    /// <summary>
    /// If inherited from, this symbol will be defined as a global symbol on any new Context.
    /// </summary>
    public interface IDefinedGlobally : ISymbolValue
    {
        public string GetDefinedName();
    }

    public class Context
    {
        public class StackContext
        {
            public enum Types
            {
                File,
                Function
            }

            private readonly Dictionary<string, ISymbolValue> Symbols;
            public readonly TypeParameterCollection TypeParams;
            public readonly int LineNumber;
            public readonly int ColNumber;
            public readonly Types Type;
            public readonly string Name;
            public Value? FunctionReturnValue = null;

            public StackContext(int line, int col, Types type, string name, TypeParameterCollection? tParams = null)
            {
                Symbols = new() {
                    { "_Context", new StringValue(name) }
                };
                LineNumber = line;
                ColNumber = col;
                Type = type;
                Name = name;
                TypeParams = tParams ?? new();
            }

            public bool HasSymbol(string symbol)
            {
                return Symbols.ContainsKey(symbol);
            }

            public ISymbolValue GetSymbol(string symbol)
            {
                if (!HasSymbol(symbol)) throw new Exception(string.Format("Failed to get variable: name '{0}' could not be found", symbol));
                return Symbols[symbol];
            }

            public void SetSymbol(string symbol, ISymbolValue value)
            {
                if (HasSymbol(symbol))
                {
                    Symbols[symbol] = value;
                }
                else
                {
                    Symbols.Add(symbol, value);
                }
            }

            public override string ToString()
            {
                string inside = "unknown";
                switch (Type)
                {
                    case Types.File:
                        inside = "file";
                        break;
                    case Types.Function:
                        inside = "function";
                        break;
                }

                return string.Format("In {0} {1} (entered at line {2}, column {3}):", inside, Name, LineNumber, ColNumber);
            }
        }

        private readonly List<StackContext> Stack;
        public Error? Error = null;

        public Context(string filename)
        {
            Stack = new() { new(0, 0, StackContext.Types.File, filename) };
        }

        /// <summary>
        /// Does the given variable exist?
        /// </summary>
        public bool HasVariable(string name)
        {
            foreach (var d in Stack)
            {
                if (d.HasSymbol(name)) return true;
            }
            return false;
        }

        /// <summary>
        /// Get the value of the given variable or throw an error. Looks from the topmost scope downwards.
        /// </summary>
        public ISymbolValue GetVariable(string name)
        {
            for (int i = Stack.Count - 1; i >= 0; i--)
            {
                if (Stack[i].HasSymbol(name)) return Stack[i].GetSymbol(name);
            }

            throw new Exception(string.Format("Failed to get variable: name '{0}' could not be found", name));
        }

        /// <summary>
        /// Get the value of the given variable. If the symbol cannot be found, return the default. Looks from the topmost scope downwards.
        /// </summary>
        public ISymbolValue GetVariableOrDefault(string name, ISymbolValue fallback)
        {
            for (int i = Stack.Count - 1; i >= 0; i--)
            {
                if (Stack[i].HasSymbol(name)) return Stack[i].GetSymbol(name);
            }

            return fallback;
        }

        /// <summary>
        /// Set the value of the given symbol, or create a new one. Sets from the topmost scope down.
        /// </summary>
        public void SetVariable(string name, ISymbolValue value)
        {
            for (int i = Stack.Count - 1; i >= 0; i--)
            {
                if (Stack[i].HasSymbol(name))
                {
                    Stack[i].SetSymbol(name, value);
                    break;
                }
            }

            CreateVariable(name, value);
        }

        /// <summary>
        /// Creates a new symbol in the topmost scope and sets it
        /// </summary>
        public void CreateVariable(string name, ISymbolValue value)
        {
            Stack[^1].SetSymbol(name, value);
        }

        public string GetErrorString()
        {
            return Error == null ? "" : ErrorToString(Error);
        }

        /// <summary>
        /// Given an error, return the error as a string using context information
        /// </summary>
        private string ErrorToString(Error error)
        {
            string str = "";
            for (int i = 0; i < Stack.Count; i++)
            {
                str += Stack[i].ToString() + Environment.NewLine;
            }

            str += error.ToString();
            return str;
        }

        /// <summary>
        /// Push a new stack context
        /// </summary>
        public void PushStackContext(int line, int col, StackContext.Types type, string name, TypeParameterCollection? typeParams = null)
        {
            Stack.Add(new(line, col, type, name, typeParams));
        }

        /// <summary>
        /// Pop the latest stack context
        /// </summary>
        public void PopStackContext()
        {
            if (Stack.Count < 2) throw new InvalidOperationException();
            Stack.RemoveAt(Stack.Count - 1);
        }

        public void SetFunctionReturnValue(Value value)
        {
            var context = Stack[^1];
            context.FunctionReturnValue = value;
        }

        public Value? GetFunctionReturnValue()
        {
            return Stack[^1].FunctionReturnValue;
        }

        /// <summary>
        /// Merge the given collection with the latest collection on the stack
        /// </summary>
        public void MergeTypeParams(TypeParameterCollection c)
        {
            Stack[^1].TypeParams.MergeWith(c);
        }

        /// <summary>
        /// From the current scope down, get any bound type parameters
        /// </summary>
        public TypeParameterCollection GetBoundTypeParams()
        {
            TypeParameterCollection c = new();
            foreach (StackContext context in Stack)
            {
                foreach (string name in context.TypeParams.GetParamerNames())
                {
                    c.SetParameter(name, context.TypeParams.GetParameter(name));
                }
            }
            return c;
        }

        public void InitialiseGlobals()
        {
            // Add all globally defined functions
            var type = typeof(IDefinedGlobally);
            foreach (IDefinedGlobally x in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface)
                .Select(o => (IDefinedGlobally)Activator.CreateInstance(o)))
            {
                CreateVariable(x.GetDefinedName(), x);
            }
        }
    }
}
