using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UglyLang.source
{
    public class Context
    {
        class StackContext
        {
            public enum Types
            {
                File,
                Function
            }

            private readonly Dictionary<string, Value> Symbols;
            public readonly int LineNumber;
            public readonly int ColNumber;
            public readonly Types Type;
            public readonly string Name;

            public StackContext(int line, int col, Types type, string name)
            {
                Symbols = new() {
                    { "_Context", new StringValue(name) }
                };
                LineNumber = line;
                ColNumber = col;
                Type = type;
                Name = name;
            }

            public bool HasSymbol(string symbol)
            {
                return Symbols.ContainsKey(symbol);
            }

            public Value GetSymbol(string symbol)
            {
                if (!HasSymbol(symbol)) throw new Exception(string.Format("Failed to get variable: name '{0}' could not be found", symbol));
                return Symbols[symbol];
            }

            public void SetSymbol(string symbol, Value value)
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
        public Value GetVariable(string name)
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
        public Value GetVariableOrDefault(string name, Value fallback)
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
        public void SetVariable(string name, Value value)
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
        public void CreateVariable(string name, Value value)
        {
            Stack[Stack.Count - 1].SetSymbol(name, value);
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
            for (int i = Stack.Count - 1; i >= 0; i--)
            {
                str += Stack[i].ToString() + Environment.NewLine;
            }

            str += error.ToString();
            return str;
        }
    }
}
