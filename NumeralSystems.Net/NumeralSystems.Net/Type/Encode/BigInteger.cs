

using System.Collections.Generic;
// ReSharper disable once CheckNamespace
using System;
using System.Linq;

namespace NumeralSystems.Net.Type.Base
{
    /// <summary>
    /// Provides static methods to perform operations based on indices with a specified base. Capable of converting integral and fractional parts of a number to and from different numeral systems.
    /// </summary>
    public partial class BigInteger
    {
        /// Converts an integral and fractional part, given as indices of a specified base, into a BigInteger representation.
        /// <param name="integral">An array of unsigned long integers representing the integral part's digits in the specified base.</param>
        /// <param name="fractional">An array of unsigned long integers representing the fractional part's digits in the specified base.</param>
        /// <param name="positive">A boolean indicating whether the resulting number should be positive or negative.</param>
        /// <param name="sourceBase">The base in which the integral and fractional parts are expressed.</param>
        /// <returns>A BigInteger representation of the number formed by the integral and fractional parts in the specified base, with consideration to its sign.</returns>
        public static System.Numerics.BigInteger FromIndicesOfBase(ulong[] integral, ulong[] fractional, bool positive, int sourceBase)
        {
            var integralPart = FromIndicesOfBase(integral, sourceBase);
            var fractionalPart = FromIndicesOfBase(fractional, sourceBase);
            var fractionalPats = ToIndicesOfBase(fractionalPart, 10);
            var result = fractionalPart / System.Numerics.BigInteger.Pow(10, fractionalPats.Length);
            var zeros = 0;
            while (fractional.Length > zeros && fractional[zeros] == 0)
            {
                zeros++;
            }
            if (zeros > 0)
            {
                result /= System.Numerics.BigInteger.Pow(10, zeros);
            }
            result += integralPart;
            return positive ? result : - result;
        }

        /// <summary>
        /// Converts an indices representation in a specified base to a BigInteger value.
        /// </summary>
        /// <param name="val">The indices representing the integral value.</param>
        /// <param name="sourceBase">The base of the numeral system represented by the indices.</param>
        /// <returns>A BigInteger value constructed from the specified indices.</returns>
        public static System.Numerics.BigInteger FromIndicesOfBase(ulong[] val, int sourceBase)
        {
            if (sourceBase < 2)
                throw new ArgumentOutOfRangeException(nameof(sourceBase), "Base must be at least 2.");
            if (val is null) throw new ArgumentNullException(nameof(val));
            if (val.Any(index => index >= (ulong)sourceBase))
                throw new ArgumentOutOfRangeException(nameof(val), "Every digit must be smaller than the source base.");

            System.Numerics.BigInteger result = 0;
            for (var i = 0; i < val.Length; i++)
            {
                result += val[i] * System.Numerics.BigInteger.Pow(new System.Numerics.BigInteger(sourceBase), val.Length - i - 1);
            }
            return result;
        }

        /// <summary>
        /// Converts a given BigInteger into its indices representation in the specified base.
        /// </summary>
        /// <param name="val">The BigInteger value to be converted.</param>
        /// <param name="destinationBase">The base to convert the BigInteger into. It must be at least 2.</param>
        /// <returns>An array of BigInteger values representing the indices of the original value in the specified base.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the destination base is less than 2.</exception>
        public static System.Numerics.BigInteger[] ToIndicesOfBase(System.Numerics.BigInteger val, int destinationBase)
        {
            if (destinationBase < 2)
                throw new ArgumentOutOfRangeException(nameof(destinationBase), "Base must be at least 2.");
            val = System.Numerics.BigInteger.Abs(val);
            if (val == 0) return new System.Numerics.BigInteger[] { 0 };

            var result = new List<System.Numerics.BigInteger>();
            while (val != 0)
            {
                var remainder = val % destinationBase;
                val /= (ulong)destinationBase;
                result.Insert(0, remainder); // Prepend operation using Insert at index 0
            }
            return result.ToArray();
        }

    }

}
