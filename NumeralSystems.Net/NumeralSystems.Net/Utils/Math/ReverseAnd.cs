using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NumeralSystems.Net.Utils
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal static partial class Math
    {
        /// <summary>
        /// Performs a logical AND operation in reverse on two boolean values.
        /// </summary>
        /// <param name="left">The first boolean value.</param>
        /// <param name="right">The second boolean value.</param>
        /// <returns>
        /// The result of the logical AND operation performed in reverse.
        /// Returns null if either <paramref name="left"/> or <paramref name="right"/> is null.
        /// Returns false if <paramref name="left"/> is false and <paramref name="right"/> is null.
        /// Throws an <see cref="InvalidOperationException"/> if <paramref name="left"/> is true and <paramref name="right"/> is null.
        /// Throws an <see cref="InvalidOperationException"/> if <paramref name="left"/> is null and <paramref name="right"/> is false.
        /// Returns null if <paramref name="left"/> is null and <paramref name="right"/> is true.
        /// Returns null if both <paramref name="left"/> and <paramref name="right"/> are false.
        /// Returns false if <paramref name="left"/> is false and <paramref name="right"/> is true.
        /// Throws an <see cref="InvalidOperationException"/> if <paramref name="left"/> is true and <paramref name="right"/> is false.
        /// Returns true if both <paramref name="left"/> and <paramref name="right"/> are true.
        /// </returns>
        public static bool? ReverseAnd(this bool? left, bool? right) => (left, right) switch
        {
            (null, null) => null,
            (false, null) => false,
            (true, null) => throw new InvalidOperationException(),
            (null, false) => throw new InvalidOperationException(),
            (null, true) => null,
            (false, false) => null,
            (false, true) => false,
            (true, false) => throw new InvalidOperationException(),
            (true, true) => true
        };

        /// <summary>
        /// Performs the reverse AND operation between two Boolean arrays.
        /// </summary>
        /// <param name="left">The left Boolean array.</param>
        /// <param name="right">The right Boolean array.</param>
        /// <returns>The result after performing the reverse AND operation element-wise on the two arrays.</returns>
        public static bool?[] ReverseAnd(this bool[] left, bool?[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("left and right must be the same length");

            var result = new bool?[left.Length];
            
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = ReverseAnd(left[i], right[i]);
            }

            return result;
        }

        /// Reverses the logical AND operation between two boolean values and returns the result.
        /// @param left The left boolean value. It can be null.
        /// @param right The right boolean value. It can be null.
        /// @return The result of the logical AND operation between the left and right values.
        /// If both values are null, the result is null.
        /// If either value is null, the result is null.
        /// If both values are false, the result is null.
        /// If the left value is false and the right value is true, the result is false.
        /// If the left value is true and the right value is false, the result is true.
        /// @throws InvalidOperationException if the left value is true and the right value is null, or if the left value is null and the right value is false.
        /// /
        public static bool?[] ReverseAnd(this bool[] left, bool[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("left and right must be the same length");

            var result = new bool?[left.Length];
            
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = ReverseAnd(left[i], right[i]);
            }

            return result;
        }

        /// <summary>
        /// Reverses the bits in each element of the left array and performs a bitwise AND operation with the corresponding elements of the right array.
        /// </summary>
        /// <param name="left">The array of nullable boolean values to reverse and AND.</param>
        /// <param name="right">The array of boolean values to AND with the reversed values.</param>
        /// <returns>An array of nullable boolean values representing the result of the bitwise AND operation.</returns>
        public static bool?[] ReverseAnd(this bool?[] left, bool[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("left and right must be the same length");

            var result = new bool?[left.Length];
            
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = ReverseAnd(left[i], right[i]);
            }

            return result;
        }

        /// Reverses the bits in the given arrays and performs a bitwise AND operation on the corresponding bits.
        /// @param left The first array of boolean values.
        /// @param right The second array of boolean values.
        /// @return An array of boolean values resulting from the bitwise AND operation on the corresponding bits of the input arrays.
        /// @throws ArgumentException if the lengths of the input arrays are not equal.
        /// /
        public static bool?[] ReverseAnd(this bool?[] left, bool?[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("left and right must be the same length");

            var result = new bool?[left.Length];
            
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = ReverseAnd(left[i], right[i]);
            }

            return result;
        }
        
        public static bool CanReverseAnd(this bool? left, bool? right) => (left, right) switch 
        {
            (true, null) => false,
            (null, false) => false,
            (true, false) => false,
            _ => true
        };

        /// <summary>
        /// Reverses the AND operation between two nullable boolean values.
        /// </summary>
        /// <param name="left">The left boolean value.</param>
        /// <param name="right">The right boolean value.</param>
        /// <returns>The result of reversing the AND operation between the two boolean values. Returns null if either value is null.</returns>
        public static bool CanReverseAnd(this bool?[] left, bool?[] right)
        {
            if (left.Length != right.Length) return false;
            return !left.Where((t, i) => !CanReverseAnd(t, right[i])).Any();
        }

        /// Reverses the logical AND operation between two boolean values and returns the result.
        /// @param left The left boolean value.
        /// @param right The right boolean value.
        /// @return The result of the logical AND operation between the left and right boolean values.
        /// /
        public static bool CanReverseAnd(this bool[] left, bool?[] right)
        {
            if (left.Length != right.Length) return false;
            return !left.Where((t, i) => !CanReverseAnd(t, right[i])).Any();
        }

        /// <summary>
        /// Reverses the logical AND operation between two nullable boolean values.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>
        /// The result of the logical AND operation between the two nullable boolean values.
        /// If both operands are null, the result is null.
        /// If any operand is null, the result is null.
        /// If both operands are false, the result is null.
        /// If the left operand is true and the right operand is null, the result is null.
        /// If the left operand is false and the right operand is null, the result is false.
        /// If the left operand is null and the right operand is true, the result is null.
        /// If the left operand is null and the right operand is false, the result is null.
        /// If the left operand is false and the right operand is true, the result is false.
        /// If the left operand is true and the right operand is false, an InvalidOperationException is thrown.</returns>
        public static bool CanReverseAnd(this bool[] left, bool[] right)
        {
            if (left.Length != right.Length) return false;
            return !left.Where((t, i) => !CanReverseAnd(t, right[i])).Any();
        }

        /// Determines whether two boolean values can be reversed and.
        /// @param left The first boolean value to check. A null value represents an unknown value.
        /// @param right The second boolean value to check. A null value represents an unknown value.
        /// @return True if the values can be reversed and; otherwise, false.
        /// /
        public static bool CanReverseAnd(this bool?[] left, bool[] right)
        {
            if (left.Length != right.Length) return false;
            return !left.Where((t, i) => !CanReverseAnd(t, right[i])).Any();
        }
    }
}