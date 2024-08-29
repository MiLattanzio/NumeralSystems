using System;

namespace NumeralSystems.Net.Utils
{
    /// <summary>
    /// A static class that provides mathematical operations.
    /// </summary>
    internal static partial class Math
    {
        /// <summary>
        /// Calculates the logical NAND (NOT AND) of two boolean values.
        /// </summary>
        /// <param name="left">The left operand boolean value.</param>
        /// <param name="right">The right operand boolean value.</param>
        /// <returns>The logical NAND of the two boolean values. Returns true if any of the values are false, otherwise returns false.</returns>
        public static bool Nand(this bool left, bool right) => (left, right) switch
        {
            (false, false) => true,
            (false, true) => true,
            (true, false) => true,
            (true, true) => false
        };

        /// <summary>
        /// Performs a bitwise NAND operation between two boolean values and returns the result.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result of the NAND operation.</returns>
        public static bool? Nand(this bool left, bool? right) => (left, right) switch
        {
            (false, null) => true,
            (true, null) => null,
            (false, false) => true,
            (false, true) => true,
            (true, false) => true,
            (true, true) => false
        };

        /// Performs the NAND operation on two boolean values.
        /// @param left The left boolean value.
        /// @param right The right boolean value.
        /// @return The result of the NAND operation as a boolean value.
        /// /
        public static bool? Nand(this bool? left, bool? right) => (left, right) switch
        {
            (null, null) => null,
            (false, null) => true,
            (true, null) => null,
            (null, false) => true,
            (null, true) => true,
            (false, false) => true,
            (false, true) => true,
            (true, false) => true,
            (true, true) => false
        };

        /// <summary>
        /// Calculates the logical NAND (NOT AND) of two boolean values.
        /// </summary>
        /// <param name="left">The left operand boolean value.</param>
        /// <param name="right">The right operand boolean value.</param>
        /// <returns>The logical NAND of the two boolean values. Returns true if any of the values are false, otherwise returns false.</returns>
        public static bool[] Nand(this bool[] left, bool[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool[left.Length];
            for (var i = 0; i < left.Length; i++)
                result[i] = left[i].Nand(right[i]);
            return result;
        }

        /// <summary>
        /// Performs a logical NAND operation between two boolean values.
        /// </summary>
        /// <param name="left">The first boolean value.</param>
        /// <param name="right">The second boolean value.</param>
        /// <returns>The result of the logical NAND operation.</returns>
        public static bool?[] Nand(this bool[] left, bool?[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool?[left.Length];
            for (var i = 0; i < left.Length; i++)
                result[i] = left[i].Nand(right[i]);
            return result;
        }

        /// <summary>
        /// Performs a Nand operation on two boolean values.
        /// </summary>
        /// <param name="left">The left operand of the Nand operation.</param>
        /// <param name="right">The right operand of the Nand operation.</param>
        /// <returns>
        /// Returns the result of the Nand operation. The result is true if either or both operands are false, and false if both operands are true.
        /// </returns>
        public static bool?[] Nand(this bool?[] left, bool?[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool?[left.Length];
            for (var i = 0; i < left.Length; i++)
                result[i] = left[i].Nand(right[i]);
            return result;
        }

        /// <summary>
        /// Calculates the logical NAND (NOT AND) of two boolean values.
        /// </summary>
        /// <param name="left">The left operand boolean value.</param>
        /// <param name="right">The right operand boolean value.</param>
        /// <returns>The logical NAND of the two boolean values. Returns true if any of the values are false, otherwise returns false.</returns>
        public static bool?[] Nand(this bool?[] left, bool[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool?[left.Length];
            for (var i = 0; i < left.Length; i++)
                result[i] = left[i].Nand(right[i]);
            return result;
        }
    }
}