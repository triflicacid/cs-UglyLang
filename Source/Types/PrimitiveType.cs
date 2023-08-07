using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace UglyLang.Source.Types
{
    public abstract class PrimitiveType : Type
    {

    }

    public class IntType : PrimitiveType
    {
        public override bool Equals(Type other)
        {
            return other is IntType;
        }

        public override bool DoesMatch(Type other)
        {
            return other is IntType or FloatType;
        }

        public override string ToString()
        {
            return AsString();
        }

        public static string AsString()
        {
            return "INT";
        }
    }

    public class FloatType : PrimitiveType
    {
        public override bool Equals(Type other)
        {
            return other is FloatType;
        }

        public override bool DoesMatch(Type other)
        {
            return other is FloatType or IntType;
        }

        public override string ToString()
        {
            return AsString();
        }

        public static string AsString()
        {
            return "FLOAT";
        }
    }

    public class StringType : PrimitiveType
    {
        public override bool Equals(Type other)
        {
            return other is StringType;
        }

        public override bool DoesMatch(Type other)
        {
            return other is StringType;
        }

        public override string ToString()
        {
            return AsString();
        }

        public static string AsString()
        {
            return "STRING";
        }
    }
}
