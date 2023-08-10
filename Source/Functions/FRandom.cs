using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyLang.Source.Types;
using UglyLang.Source.Values;

namespace UglyLang.Source.Functions
{
    /// <summary>
    /// Returns a random floating number in a range, depending on the parameters passed.
    /// </summary>
    public class FRandom : Function
    {

        private static readonly Random Generator = new();
        private static readonly List<UnresolvedType[]> Arguments = new()
        {
            Array.Empty<UnresolvedType>(),
            new UnresolvedType[] { ResolvedType.Float },
            new UnresolvedType[] { ResolvedType.Float, ResolvedType.Float},
        };

        public FRandom() : base(Arguments, ResolvedType.Int) { }

        protected override Value CallOverload(Context context, int _, List<Value> arguments, TypeParameterCollection c)
        {
            double n;
            if (arguments.Count == 0) // Range: [0,1)
            {
                n = Generator.NextDouble();
            }
            else if (arguments.Count == 1) // Range: [0,max)
            {
                double max = ((FloatValue)arguments[0]).Value;
                n = Generator.NextDouble() * max;
            }
            else if (arguments.Count == 2) // Range: [min,max)
            {
                double min = ((FloatValue)arguments[0]).Value;
                double max = ((FloatValue)arguments[1]).Value;
                n = min + Generator.NextDouble() * (max - min);
            }
            else
            {
                throw new InvalidOperationException();
            }

            return new FloatValue(n);
        }
    }
}
