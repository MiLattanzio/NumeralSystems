using System;
using System.Globalization;
using System.Linq;
using NumeralSystems.Net.Utils;

// ReSharper disable once CheckNamespace
namespace NumeralSystems.Net.Type.Base
{
    public partial class Decimal
    {
        /// <summary>
        /// Converts a decimal value to its indices representation in a specified base.
        /// </summary>
        /// <param name="val">The decimal value to convert.</param>
        /// <param name="destinationBase">The base to convert to.</param>
        /// <returns>A tuple containing the integral part, fractional part, and a boolean indicating if the value is positive.</returns>
        public static (ulong[] Integral, ulong[] Fractional, bool positive) ToIndicesOfBase(decimal val, int destinationBase)
        {
            var result = PositionalNotation.FromDecimal(val, destinationBase);
            return (
                result.Integral.Select(x => (ulong)x).ToArray(),
                result.Fractional.Select(x => (ulong)x).ToArray(),
                result.Positive);
        }

        /// <summary>
        /// Converts a decimal value to positional digits with an explicit fractional digit limit.
        /// </summary>
        /// <param name="val">The decimal value to convert.</param>
        /// <param name="destinationBase">The destination base.</param>
        /// <param name="maxFractionalDigits">The maximum number of generated fractional digits.</param>
        /// <returns>
        /// The integral and fractional digits, the sign, and whether the fractional conversion terminated
        /// before reaching <paramref name="maxFractionalDigits"/>.
        /// </returns>
        public static (
            ulong[] Integral,
            ulong[] Fractional,
            bool Positive,
            bool Exact) ToIndicesOfBase(decimal val, int destinationBase, int maxFractionalDigits)
        {
            var result = PositionalNotation.FromDecimal(val, destinationBase, maxFractionalDigits);
            return (
                result.Integral.Select(x => (ulong)x).ToArray(),
                result.Fractional.Select(x => (ulong)x).ToArray(),
                result.Positive,
                result.Exact);
        }

        /// <summary>
        /// Converts indices representation in a specified base to a decimal value.
        /// </summary>
        /// <param name="integral">The integral part indices.</param>
        /// <param name="fractional">The fractional part indices.</param>
        /// <param name="positive">Indicates if the value is positive.</param>
        /// <param name="sourceBase">The base of the indices.</param>
        /// <returns>The decimal value.</returns>
        public static decimal FromIndicesOfBase(ulong[] integral, ulong[] fractional, bool positive, int sourceBase)
        {
            return PositionalNotation.ToDecimal(
                integral.Select(x => checked((int)x)),
                fractional.Select(x => checked((int)x)),
                positive,
                sourceBase);
        }

        /// <summary>
        /// Converts a double value to a decimal value.
        /// </summary>
        /// <param name="val">The double value to convert.</param>
        /// <returns>The decimal value.</returns>
        public static decimal From(double val)
        {
            //Using string to avoid precision loss
            return decimal.Parse(val.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
        }
    }
}
