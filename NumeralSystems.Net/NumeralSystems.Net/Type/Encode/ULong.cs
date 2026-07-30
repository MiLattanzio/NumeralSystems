using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

// ReSharper disable once CheckNamespace
namespace NumeralSystems.Net.Type.Base
{
    public sealed partial class ULong
    {
        /// <summary>
        /// Converts an unsigned long value to its indices representation in a specified base.
        /// </summary>
        /// <param name="val">The unsigned long value to convert.</param>
        /// <param name="destinationBase">The base to convert to.</param>
        /// <returns>An array of unsigned long representing the indices in the specified base.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the destination base is less than 2.</exception>
        public static ulong[] ToIndicesOfBase(ulong val, int destinationBase)
        {
            if (destinationBase < 2)
                throw new ArgumentOutOfRangeException(nameof(destinationBase), "Base must be at least 2.");
            if (val == 0) return new ulong[] { 0 };

            List<ulong> result = new List<ulong>();
            while (val != 0)
            {
                ulong remainder = val % (ulong)destinationBase;
                val /= (ulong)destinationBase;
                result.Insert(0, remainder); // Prepend operation using Insert at index 0
            }
            return result.ToArray();
        }

        /// <summary>
        /// Converts indices representation in a specified base to an unsigned long value.
        /// </summary>
        /// <param name="val">The array of unsigned long representing the indices.</param>
        /// <param name="sourceBase">The base of the indices.</param>
        /// <returns>The unsigned long value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the source base is less than 2.</exception>
        public static ulong FromIndicesOfBase(ulong[] val, int sourceBase)
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
            return (ulong)result;
        }
    }
}
