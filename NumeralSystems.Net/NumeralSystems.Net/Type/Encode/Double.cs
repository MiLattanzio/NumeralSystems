using System.Linq;

using NumeralSystems.Net.Utils;

// ReSharper disable once CheckNamespace
namespace NumeralSystems.Net.Type.Base
{
    public sealed partial class Double
    {
        /// <summary>
        /// Converts a double value to its indices representation in a specified base.
        /// </summary>
        /// <param name="val">The double value to convert.</param>
        /// <param name="destinationBase">The base to convert to.</param>
        /// <returns>A tuple containing the integral part, fractional part, and a boolean indicating if the value is positive.</returns>
        public static (ulong[] Integral, ulong[] Fractional, bool positive) ToIndicesOfBase(double val, int destinationBase)
        {
            if (double.IsNaN(val) || double.IsInfinity(val))
                throw new System.ArgumentOutOfRangeException(nameof(val), "Only finite values can be converted.");

            return Decimal.ToIndicesOfBase(Decimal.From(val), destinationBase);
        }

        /// <summary>
        /// Converts indices representation in a specified base to a double value.
        /// </summary>
        /// <param name="integral">The integral part indices.</param>
        /// <param name="fractional">The fractional part indices.</param>
        /// <param name="positive">Indicates if the value is positive.</param>
        /// <param name="sourceBase">The base of the indices.</param>
        /// <returns>The double value.</returns>
        public static double FromIndicesOfBase(ulong[] integral, ulong[] fractional, bool positive, int sourceBase)
        {
            return PositionalNotation.ToDouble(integral, fractional, positive, sourceBase);
        }
    }
}
