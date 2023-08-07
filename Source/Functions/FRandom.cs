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
        private static readonly List<Types.Type[]> ArgumentType = new()
        {
            Array.Empty<Types.Type>(),
            new Types.Type[] { new FloatType() },
            new Types.Type[] { new FloatType(), new FloatType()},
        };

        public FRandom() : base(ArgumentType, new IntType()) { }

        protected override Value CallOverload(Context context, int _, List<Value> arguments)
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
