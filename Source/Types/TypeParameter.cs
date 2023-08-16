namespace UglyLang.Source.Types
{
    public class TypeParameter : Type
    {
        public readonly string Symbol;

        public TypeParameter(string symbol)
        {
            Symbol = symbol;
        }

        public override bool Equals(Type other)
        {
            return other is TypeParameter;
        }

        public override bool DoesMatch(Type other, TypeParameterCollection? coll)
        {
            return true; // Parameter could be anything, so accept
        }

        public override string ToString()
        {
            return Symbol;
        }

        public override bool IsParameterised()
        {
            return true;
        }

        public override List<TypeParameter> GetTypeParameters()
        {
            return new() { this };
        }

        public override TypeParameterCollection MatchParametersAgainst(Type t)
        {
            TypeParameterCollection col = new();
            col.SetParameter(Symbol, t);
            return col;
        }

        public override Type ResolveParametersAgainst(TypeParameterCollection col)
        {
            return col.HasParameter(Symbol) ? col.GetParameter(Symbol) : this;
        }
    }

    public class TypeParameterCollection
    {
        private Dictionary<string, Type> Dict = new();

        public bool HasParameter(string name)
        {
            return Dict.ContainsKey(name);
        }

        public Type GetParameter(string name)
        {
            return Dict[name];
        }

        public void SetParameter(string name, Type type)
        {
            if (!HasParameter(name)) Dict.Add(name, type);
            else Dict[name] = type;
        }

        public List<string> GetParamerNames()
        {
            return Dict.Keys.ToList();
        }

        public void Clear()
        {
            Dict.Clear();
        }

        /// <summary>
        /// Same as t1.DoesMatch(t2) but handles type parameters
        /// </summary>
        public bool DoesMatch(Type t1, Type t2)
        {
            if (t1 is TypeParameter tp1)
            {
                if (HasParameter(tp1.Symbol))
                {
                    t1 = GetParameter(tp1.Symbol);
                }
                else
                {
                    return false;
                }
            }

            if (t2 is TypeParameter tp2)
            {
                if (HasParameter(tp2.Symbol))
                {
                    t2 = GetParameter(tp2.Symbol);
                }
                else
                {
                    return false;
                }
            }

            return t1.DoesMatch(t2);
        }

        public bool DoesMatch(Type t1, Type[] t2s)
        {
            if (t1 is TypeParameter tp1)
            {
                if (HasParameter(tp1.Symbol))
                {
                    t1 = GetParameter(tp1.Symbol);
                }
                else
                {
                    return false;
                }
            }

            for (int i = 0; i < t2s.Length; i++)
            {
                if (t2s[i] is TypeParameter tp2)
                {
                    if (HasParameter(tp2.Symbol))
                    {
                        t2s[i] = GetParameter(tp2.Symbol);

                        if (t1.DoesMatch(t2s[i])) return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return false;
        }

        public void MergeWith(TypeParameterCollection col)
        {
            foreach (string name in col.GetParamerNames())
            {
                SetParameter(name, col.GetParameter(name));
            }
        }
    }
}
