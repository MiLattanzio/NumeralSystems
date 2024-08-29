using System;
using System.Diagnostics.CodeAnalysis;

namespace NumeralSystems.Net.Utils
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal static partial class Math
    {
        /// <summary>
        /// Performs the logical XOR operation on two boolean values.
        /// </summary>
        /// <param name="left">The left operand of the XOR operation.</param>
        /// <param name="right">The right operand of the XOR operation.</param>
        /// <returns>The result of the XOR operation between the left and right operands. The result is null if any of the operands is null.</returns>
        public static bool? Xor(this bool left, bool? right) => (left, right) switch
        {
            (false, null) => null,
            (true, null) => null,
            (false, false) => false,
            (false, true) => true,
            (true, false) => true,
            (true, true) => false
        };

        /// <summary>
        /// Computes the exclusive OR (XOR) of two boolean values.
        /// </summary>
        /// <param name="left">The first boolean value.</param>
        /// <param name="right">The second boolean value.</param>
        /// <returns>The XOR result of the two boolean values.</returns>
        public static bool? Xor(this bool? left, bool? right) => (left, right) switch
        {
            (null, null) => null,
            (false, null) => null,
            (true, null) => null,
            (null, false) => null,
            (null, true) => null,
            (false, false) => false,
            (false, true) => true,
            (true, false) => true,
            (true, true) => false
        };

        /// <summary>
        /// Performs an exclusive OR (XOR) operation.
        /// </summary>
        /// <param name="left">The left operand of the XOR operation.</param>
        /// <param name="right">The right operand of the XOR operation.</param>
        /// <returns>The result of the XOR operation.</returns>
        public static bool? Xor(this bool? left, bool right) => (left, right) switch
        {
            (null, false) => null,
            (null, true) => null,
            (false, false) => false,
            (false, true) => true,
            (true, false) => true,
            (true, true) => false
        };

        /// <summary>
        /// Calculates the exclusive OR (XOR) of two boolean values.
        /// </summary>
        /// <param name="left">The first boolean value.</param>
        /// <param name="right">The second boolean value.</param>
        /// <returns>The result of the XOR operation. Returns null if any of the input values are null.</returns>
        public static bool Xor(this bool left, bool right) => left ^ right;

        /// <summary>
        /// Performs an exclusive OR (XOR) operation between a boolean value and a nullable boolean value.
        /// </summary>
        /// <param name="left">The boolean value.</param>
        /// <param name="right">The nullable boolean value.</param>
        /// <returns>
        /// - Returns null if both the boolean value and the nullable boolean value are null.
        /// - Returns null if the boolean value is false and the nullable boolean value is null.
        /// - Returns null if the boolean value is true and the nullable boolean value is null.
        /// - Returns false if the boolean value is false and the nullable boolean value is false.
        /// - Returns true if the boolean value is false and the nullable boolean value is true.
        /// - Returns true if the boolean value is true and the nullable boolean value is false.
        /// - Returns false if the boolean value is true and the nullable boolean value is true.
        /// </returns>
        public static bool?[] Xor(this bool[] left, bool?[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool?[left.Length];
            for (var i = 0; i < left.Length; i++)
                result[i] = left[i].Xor(right[i]);
            return result;
        }

        /// <summary>
        /// Performs a logical XOR operation between a bool value and a nullable bool value.
        /// </summary>
        /// <param name="left">The bool value to XOR.</param>
        /// <param name="right">The nullable bool value to XOR.</param>
        /// <returns>The result of the XOR operation. Returns null if any of the input values is null.</returns>
        public static bool[] Xor(this bool[] left, bool[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool[left.Length];
            for (var i = 0; i < left.Length; i++)
                result[i] = left[i].Xor(right[i]);
            return result;
        }

        /// <summary>
        /// Performs a logical XOR operation on two arrays of nullable boolean values.
        /// </summary>
        /// <param name="left">The left array of boolean values.</param>
        /// <param name="right">The right array of boolean values.</param>
        /// <returns>An array of nullable boolean values resulting from performing the XOR operation element-wise on the input arrays.</returns>
        /// <exception cref="ArgumentException">Thrown when the lengths of the input arrays are not equal.</exception>
        public static bool?[] Xor(this bool?[] left, bool?[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool?[left.Length];
            for (var i = 0; i < left.Length; i++)
                result[i] = left[i].Xor(right[i]);
            return result;
        }

        /// <summary>
        /// Performs the logical XOR operation on two boolean values.
        /// </summary>
        /// <param name="left">The left operand of the XOR operation.</param>
        /// <param name="right">The right operand of the XOR operation.</param>
        /// <returns>The result of the XOR operation between the left and right operands. The result is null if any of the operands is null.</returns>
        public static bool?[] Xor(this bool?[] left, bool[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool?[left.Length];
            for (var i = 0; i < left.Length; i++)
                result[i] = left[i].Xor(right[i]);
            return result;
        }
    }
}