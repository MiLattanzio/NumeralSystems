using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace NumeralSystems.Net.Type.Base
{
    public sealed partial class Float
    {
        /// <summary>
        /// Converts a float value to its indices representation in a specified base.
        /// </summary>
        /// <param name="val">The float value to convert.</param>
        /// <param name="destinationBase">The base to convert to.</param>
        /// <returns>A tuple containing the integral part, fractional part, and a boolean indicating if the value is positive.</returns>
        public static (uint[] Integral, uint[] Fractional, bool positive) ToIndicesOfBase(float val, int destinationBase)
        {
            var result = Double.ToIndicesOfBase(val, destinationBase);
            return (
                result.Integral.Select(x => checked((uint)x)).ToArray(),
                result.Fractional.Select(x => checked((uint)x)).ToArray(),
                result.positive);
        }

        /// <summary>
        /// Converts indices representation in a specified base to a float value.
        /// </summary>
        /// <param name="integral">The integral part indices.</param>
        /// <param name="fractional">The fractional part indices.</param>
        /// <param name="positive">Indicates if the value is positive.</param>
        /// <param name="sourceBase">The base of the indices.</param>
        /// <returns>The float value.</returns>
        public static float FromIndicesOfBase(uint[] integral, uint[] fractional, bool positive, int sourceBase)
        {
            var result = Double.FromIndicesOfBase(
                integral.Select(x => (ulong)x).ToArray(),
                fractional.Select(x => (ulong)x).ToArray(),
                positive,
                sourceBase);
            return System.Convert.ToSingle(result);
        }
    }
}
