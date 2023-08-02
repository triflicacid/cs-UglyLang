using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UglyLang.Source.Functions;
using UglyLang.Source.Values;

namespace UglyLang.Source
{
    public class Context
    {
        public class StackContext
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
            for (int i = 0; i < Stack.Count; i++)
            {
                str += Stack[i].ToString() + Environment.NewLine;
            }

            str += error.ToString();
            return str;
        }

        public void PushStackContext(int line, int col, StackContext.Types type, string name)
        {
            Stack.Add(new(line, col, type, name));
        }

        public void PopStackContext()
        {
            if (Stack.Count < 2) throw new InvalidOperationException();
            Stack.RemoveAt(Stack.Count - 1);
        }

        public void InitialiseBuiltinFunctions()
        {
            var context = Stack[0];
            context.SetSymbol("CONCAT", new BuiltinFuncValue(new FConcat()));
            context.SetSymbol("RANDOM", new BuiltinFuncValue(new FRandom()));
            context.SetSymbol("SUCC", new BuiltinFuncValue(new Functions.Maths.FSucc()));
            context.SetSymbol("TYPE", new BuiltinFuncValue(new FType()));

            context.SetSymbol("EQ", new BuiltinFuncValue(new Functions.Comparative.FEq()));
            context.SetSymbol("GT", new BuiltinFuncValue(new Functions.Comparative.FGt()));
            context.SetSymbol("GE", new BuiltinFuncValue(new Functions.Comparative.FGe()));
            context.SetSymbol("LT", new BuiltinFuncValue(new Functions.Comparative.FLt()));
            context.SetSymbol("LE", new BuiltinFuncValue(new Functions.Comparative.FLe()));

            context.SetSymbol("ADD", new BuiltinFuncValue(new Functions.Maths.FAdd()));
            context.SetSymbol("SUB", new BuiltinFuncValue(new Functions.Maths.FSub()));
            context.SetSymbol("DIV", new BuiltinFuncValue(new Functions.Maths.FDiv()));
            context.SetSymbol("MOD", new BuiltinFuncValue(new Functions.Maths.FMod()));
            context.SetSymbol("MUL", new BuiltinFuncValue(new Functions.Maths.FMul()));
            context.SetSymbol("EXP", new BuiltinFuncValue(new Functions.Maths.FExp()));
            context.SetSymbol("NEG", new BuiltinFuncValue(new Functions.Maths.FNeg()));
        }
    }
}
