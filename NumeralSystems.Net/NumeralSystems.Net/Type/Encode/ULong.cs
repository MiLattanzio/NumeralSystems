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
        /// <exception cref="ArgumentException">Thrown when the destination base is less than or equal to 0.</exception>
        public static ulong[] ToIndicesOfBase(ulong val, int destinationBase)
        {
            if (destinationBase <= 0)
            {
                throw new ArgumentException("Destination base must be greater than 0");
            }
            if (destinationBase == 1)
            {
                if (val == 0) return new ulong[] { 0 }; // Special handling for 0 in base 1
                var array = new ulong[val];
                for (ulong i = 0; i < val; i++)
                {
                    array[i] = 1;
                }
                return array;
            }
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
        /// <exception cref="ArgumentException">Thrown when the source base is less than or equal to 0.</exception>
        public static ulong FromIndicesOfBase(ulong[] val, int sourceBase)
        {
            if (sourceBase <= 0)
            {
                throw new ArgumentException("Source base must be greater than 0");
            }
            if (sourceBase == 1)
            {
                return (ulong)new System.Numerics.BigInteger(val.Length);
            }

            System.Numerics.BigInteger result = 0;
            for (var i = 0; i < val.Length; i++)
            {
                result += val[i] * System.Numerics.BigInteger.Pow(new System.Numerics.BigInteger(sourceBase), val.Length - i - 1);
            }
            return (ulong)result;
        }
    }
}