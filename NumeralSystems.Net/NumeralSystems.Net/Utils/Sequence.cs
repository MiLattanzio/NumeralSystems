using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace NumeralSystems.Net.Utils
{
    /// <summary>
    /// This class provides utility methods for working with sequences of different types.
    /// </summary>
    public static class Sequence
    {
        /// <summary>
        /// Returns an enumerable sequence of identity values at the given index.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the identity list.</typeparam>
        /// <param name="identity">The list of identity values.</param>
        /// <param name="index">The index for which to get the identity sequence.</param>
        /// <returns>An enumerable sequence of identity values.</returns>
        public static IEnumerable<T> SequenceOfIdentityAtIndex<T>(List<T> identity, int index)
        { 
            IEnumerable<T> result = new List<T>();
            while (index != 0)
            {
                index = System.Math.DivRem(index, identity.Count(), out var remainder);
                result = result.Prepend(identity[remainder]);
            }
            return result;
        }

        /// <summary>
        /// Returns an enumerable sequence of identity values of a given size.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the identity list.</typeparam>
        /// <param name="identity">The list of identity values.</param>
        /// <param name="size">The size of the identity sequence.</param>
        /// <returns>An enumerable sequence of identity values.</returns>
        public static IEnumerable<IEnumerable<T>> IdentityEnumerableOfSize<T>(List<T> identity, int size)
        {
            var idx = 0;
            while (idx < size)
            {
                if (idx < identity.Count())
                {
                    yield return new List<T> { identity[idx] };
                }
                else
                {
                    yield return SequenceOfIdentityAtIndex(identity, idx);
                }
                idx++;
            }
            
            
        }

        /// <summary>
        /// Provides functionality to determine if a given value falls within a specified range.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The minimum value of the range (inclusive).</param>
        /// <param name="max">The maximum value of the range (inclusive).</param>
        /// <returns>
        /// True if the value falls within the specified range, otherwise false.
        /// </returns>
        public static IEnumerable<uint> Range(uint start, uint end)
        {
            for (var i = start; i <= end; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Counts the number of items in the given sequence and returns it as an unsigned integer.
        /// </summary>
        /// <param name="sequence">The sequence of items.</param>
        /// <returns>The count of items in the sequence as an unsigned integer.</returns>
        public static uint CountToUInt(IEnumerable sequence)
        {
            var count = 0u;
            foreach (var value in sequence)
            {
                count++;
            }
            return count;
        }

        /// <summary>
        /// Counts the number of elements in the given sequence and returns the count as an ulong.
        /// </summary>
        /// <param name="sequence">The sequence to count the elements of.</param>
        /// <returns>The count of elements in the sequence.</returns>
        public static ulong CountToULong(IEnumerable sequence)
        {
            var count = 0ul;
            foreach (var value in sequence)
            {
                count++;
            }
            return count;
        }

        /// <summary>
        /// Generates a sequence of unsigned integers starting from the specified start value and ending at the specified end value.
        /// </summary>
        /// <param name="start">The starting value of the sequence.</param>
        /// <param name="end">The ending value of the sequence.</param>
        /// <returns>An enumerable sequence of unsigned integers from start to end.</returns>
        public static IEnumerable<BigInteger> Range(BigInteger start, BigInteger end)
        {
            for (var i = start; i <= end; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Generates a sequence of unsigned integers within a specified range.
        /// </summary>
        /// <param name="start">The starting value of the range.</param>
        /// <param name="end">The ending value of the range.</param>
        /// <returns>An IEnumerable of unsigned integers within the specified range.</returns>
        public static IEnumerable<ulong> Range(ulong start, ulong end)
        {
            for (var i = start; i <= end; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Calculates the number of permutations of a given size from a given identity.
        /// </summary>
        /// <param name="identity">The value representing the number of elements in the identity.</param>
        /// <param name="size">The size of the permutation.</param>
        /// <param name="repetition">A flag indicating whether repetition is allowed in the permutation.</param>
        /// <returns>The number of permutations of the given size from the given identity.</returns>
        public static BigInteger PermutationsCount(BigInteger identity, BigInteger size, bool repetition = false)
            => repetition ? Pow(identity, size) : (Factorial(identity) / Factorial(identity -size));

        /// <summary>
        /// Calculates the number of permutations given an identity, size, and repetition option.
        /// </summary>
        /// <param name="identity">The number of distinct elements in the identity set.</param>
        /// <param name="size">The number of elements selected from the identity set.</param>
        /// <param name="repetition">Indicates whether repetition is allowed (default: false).</param>
        /// <returns>The number of permutations.</returns>
        public static ulong PermutationsCount(ulong identity, ulong size, bool repetition = false)
            => repetition ? Pow(identity, size) : (Factorial(identity) / Factorial(identity -size));

        /// <summary>
        /// Calculates the number of permutations for a given identity and size, with an option to allow repetition.
        /// </summary>
        /// <param name="identity">The total number of distinct objects.</param>
        /// <param name="size">The number of objects to choose.</param>
        /// <param name="repetition">If set to true, allows repetitions in permutations; otherwise, repetitions are not allowed.</param>
        /// <returns>The count of permutations based on the specified parameters.</returns>
        public static int PermutationsCount(int identity, int size, bool repetition = false)
            => repetition ? System.Convert.ToInt32(System.Math.Pow(identity, size)) : (Factorial(identity) / Factorial(identity -size));

        /// <summary>
        /// Calculates the number of permutations for a given identity and size.
        /// </summary>
        /// <param name="identity">The identity value.</param>
        /// <param name="size">The size of the permutation.</param>
        /// <param name="repetition">Optional. Specifies whether repetition is allowed in the permutation. Default is false.</param>
        /// <returns>The number of permutations.</returns>
        public static uint PermutationsCount(uint identity, uint size, bool repetition = false)
        {
            try
            {
                if (repetition)
                {
                    return System.Convert.ToUInt32(System.Math.Pow(identity, size));
                }
                return  (Factorial(identity) / Factorial(identity -size));
            }
            catch (OverflowException e)
            {
                Console.WriteLine(e);
                return uint.MaxValue;
            }
        }

        /// <summary>
        /// Calculates the number of combinations of a given size that can be formed from a given set of elements.
        /// </summary>
        /// <param name="identity">The number of elements in the set</param>
        /// <param name="size">The size of each combination</param>
        /// <param name="repetition">Specifies whether repetition is allowed in the combinations</param>
        /// <returns>The number of combinations</returns>
        public static int CombinationsCount(int identity, int size, bool repetition = false)
            => repetition
                ? (int)(Factorial(identity + size - 1) / (Factorial(size) * Factorial(identity - 1)))
                : (int)(Factorial(identity) / (Factorial(size) * Factorial(identity - size)));

        /// <summary>
        /// Calculates the exponentiation of the base number by the exponent using binary exponentiation algorithm.
        /// </summary>
        /// <param name="baseNumber">The base number.</param>
        /// <param name="exponent">The exponent.</param>
        /// <returns>The result of the exponentiation.</returns>
        private static BigInteger Pow(BigInteger baseNumber, BigInteger exponent)
        {
            var result = BigInteger.One;
            while (exponent > 0)
            {
                if (exponent % 2 == 1)
                {
                    result *= baseNumber;
                }
                baseNumber *= baseNumber;
                exponent /= 2;
            }
            return result;
        }

        /// <summary>
        /// Calculates the result of raising a base number to an exponent.
        /// </summary>
        /// <param name="baseNumber">The base number.</param>
        /// <param name="exponent">The exponent.</param>
        /// <returns>The result of raising the base number to the exponent.</returns>
        private static ulong Pow(ulong baseNumber, ulong exponent)
        {
            var result = 1ul;
            while (exponent > 0)
            {
                if (exponent % 2 == 1)
                {
                    result *= baseNumber;
                }
                baseNumber *= baseNumber;
                exponent /= 2;
            }
            return result;
        }

        /// <summary>
        /// Calculates the factorial of a given number.
        /// </summary>
        /// <param name="n">The number for which to calculate the factorial.</param>
        /// <returns>The factorial of the given number.</returns>
        private static int Factorial(int n)
        {
            if (n == 0) return 1;
            return n * Factorial(n - 1);
        }

        /// <summary>
        /// Calculates the factorial of a given number.
        /// </summary>
        /// <param name="n">The number for which to calculate the factorial.</param>
        /// <returns>The factorial of the given number.</returns>
        private static uint Factorial(uint n)
        {
            if (n == 0) return 1;
            return n * Factorial(n - 1);
        }

        /// <summary>
        /// Calculates the factorial of a given number.
        /// </summary>
        /// <param name="n">The number for which to calculate the factorial.</param>
        /// <returns>The factorial of the given number.</returns>
        private static BigInteger Factorial(BigInteger n)
        {
            if (n == 0) return 1;
            return n * Factorial(n - 1);
        }

        /// <summary>
        /// Calculates the factorial of a given number.
        /// </summary>
        /// <param name="n">The number for which to calculate the factorial.</param>
        /// <returns>The factorial of the given number.</returns>
        private static ulong Factorial(ulong n)
        {
            if (n == 0) return 1;
            return n * Factorial(n - 1);
        }


        /// <summary>
        /// Calculates the factorial of the given number.
        /// </summary>
        /// <param name="n">The number to calculate the factorial of.</param>
        /// <returns>The factorial of the given number.</returns>
        private static long Factorial(long n)
        {
            if (n == 0) return 1;
            return n * Factorial(n - 1);
        }


        /// <summary>
        /// Groups the elements of the array into subarrays of the specified size.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the array.</typeparam>
        /// <param name="sequence">The array of elements to be grouped.</param>
        /// <param name="count">The size of each group.</param>
        /// <returns>A two-dimensional array where each subarray contains elements of the specified size.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the length of the array is not divisible by the specified group size.</exception>
        public static T[][] Group<T>(this T[] sequence, int count)
        {
            if (sequence.Length % count != 0) throw new InvalidOperationException();
            var array = new T[sequence.Length / count][];
            for (var i = 0; i < sequence.Length / count; i++)
            {
                array[i] = sequence.Skip(i * count).Take(count).ToArray();
            }
            return array;
        }
    }
}