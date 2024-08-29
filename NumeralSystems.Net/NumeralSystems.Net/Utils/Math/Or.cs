using System;
using System.Diagnostics.CodeAnalysis;

namespace NumeralSystems.Net.Utils
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal static partial class Math
    {
        /// <summary>
        /// Performs a logical OR operation on booleans.
        /// </summary>
        /// <param name="left">The first boolean value.</param>
        /// <param name="right">The second boolean value.</param>
        /// <returns>The result of the logical OR operation. If both <paramref name="left"/> and <paramref name="right"/> are <c>null</c>, the result is <c>null</c>. If either <paramref name="left"/> or <paramref name="right"/> is <c>true</c>, the result is <c>true</c>. Otherwise, the result is <c>false</c>.</returns>
        public static bool? Or(this bool? left, bool? right) => (left, right) switch
        {
            (null, null) => null,
            (false, null) => null,
            (true, null) => true,
            (null, false) => null,
            (null, true) => true,
            (false, false) => false,
            (false, true) => true,
            (true, false) => true,
            (true, true) => true
        };

        /// <summary>
        /// Performs a logical OR operation between two nullable boolean values.
        /// </summary>
        /// <param name="left">The left nullable boolean value.</param>
        /// <param name="right">The right nullable boolean value.</param>
        /// <returns>
        /// - Returns <see langword="null"/> if both <paramref name="left"/> and <paramref name="right"/> are <see langword="null"/>.
        /// - Returns <see langword="null"/> if <paramref name="left"/> is <see langword="false"/> and <paramref name="right"/> is <see langword="null"/>.
        /// - Returns <see langword="true"/> if <paramref name="left"/> is <see langword="true"/> and <paramref name="right"/> is <see langword="null"/>.
        /// - Returns <see langword="null"/> if <paramref name="left"/> is <see langword="null"/> and <paramref name="right"/> is <see langword="false"/>.
        /// - Returns <see langword="true"/> if <paramref name="left"/> is <see langword="null"/> and <paramref name="right"/> is <see langword="true"/>.
        /// - Returns <see langword="false"/> if <paramref name="left"/> is <see langword="false"/> and <paramref name="right"/> is <see langword="false"/>.
        /// - Returns <see langword="true"/> if <paramref name="left"/> is <see langword="false"/> and <paramref name="right"/> is <see langword="true"/>.
        /// - Returns <see langword="true"/> if <paramref name="left"/> is <see langword="true"/> and <paramref name="right"/> is <see langword="false"/>.
        /// - Returns <see langword="true"/> if both <paramref name="left"/> and <paramref name="right"/> are <see langword="true"/>.
        /// </returns>
        public static bool? Or(this bool left, bool? right) => (left, right) switch
        {
            (false, null) => null,
            (true, null) => true,
            (false, false) => false,
            (false, true) => true,
            (true, false) => true,
            (true, true) => true
        };

        /// <summary>
        /// Performs a logical OR operation between two nullable boolean values.
        /// </summary>
        /// <param name="left">The first nullable boolean value.</param>
        /// <param name="right">The second nullable boolean value.</param>
        /// <returns>
        /// - If both <paramref name="left"/> and <paramref name="right"/> are <c>null</c>, returns <c>null</c>.
        /// - If <paramref name="left"/> is <c>false</c> and <paramref name="right"/> is <c>null</c>, returns <c>null</c>.
        /// - If <paramref name="left"/> is <c>true</c> and <paramref name="right"/> is <c>null</c>, returns <c>true</c>.
        /// - If <paramref name="left"/> is <c>null</c> and <paramref name="right"/> is <c>false</c>, returns <c>null</c>.
        /// - If <paramref name="left"/> is <c>null</c> and <paramref name="right"/> is <c>true</c>, returns <c>true</c>.
        /// - If both <paramref name="left"/> and <paramref name="right"/> are <c>false</c>, returns <c>false</c>.
        /// - If <paramref name="left"/> is <c>false</c> and <paramref name="right"/> is <c>true</c>, returns <c>true</c>.
        /// - If <paramref name="left"/> is <c>true</c> and <paramref name="right"/> is <c>false</c>, returns <c>true</c>.
        /// - If both <paramref name="left"/> and <paramref name="right"/> are <c>true</c>, returns <c>true</c>.
        /// </returns>
        public static bool[] Or(this bool[] left, bool[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool[left.Length];
            for (var i = 0; i < left.Length; i++)
                 result[i] = left[i].Or(right[i]);
            return result;
        }
        
        public static bool?[] Or(this bool?[] left, bool?[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool?[left.Length];
            for (var i = 0; i < left.Length; i++)
                result[i] = left[i].Or(right[i]);
            return result;
        }

        /// Performs a logical OR operation between two nullable boolean values.
        /// Returns the result of the OR operation.
        /// If both values are null, the result is null.
        /// If one value is null and the other is false, the result is null.
        /// If one value is null and the other is true, the result is true.
        /// If both values are false, the result is false.
        /// If one value is false and the other is true, the result is true.
        /// If both values are true, the result is true.
        /// @param left The left-hand side nullable boolean value.
        /// @param right The right-hand side nullable boolean value.
        /// @return A nullable boolean value representing the result of the OR operation.
        /// /
        public static bool?[] Or(this bool?[] left, bool[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool?[left.Length];
            for (var i = 0; i < left.Length; i++)
                result[i] = left[i].Or(right[i]);
            return result;
        }

        /// <summary>
        /// Performs a bitwise OR operation on two boolean values and returns the result.
        /// </summary>
        /// <param name="left">The first boolean value.</param>
        /// <param name="right">The second boolean value.</param>
        /// <returns>
        /// - If both <paramref name="left"/> and <paramref name="right"/> are null, returns null.
        /// - If <paramref name="left"/> is false and <paramref name="right"/> is null, returns null.
        /// - If <paramref name="left"/> is true and <paramref name="right"/> is null, returns true.
        /// - If <paramref name="left"/> is null and <paramref name="right"/> is false, returns null.
        /// - If <paramref name="left"/> is null and <paramref name="right"/> is true, returns true.
        /// - If <paramref name="left"/> is false and <paramref name="right"/> is false, returns false.
        /// - If <paramref name="left"/> is false and <paramref name="right"/> is true, returns true.
        /// - If <paramref name="left"/> is true and <paramref name="right"/> is false, returns true.
        /// - If both <paramref name="left"/> and <paramref name="right"/> are true, returns true.
        /// </returns>
        public static bool?[] Or(this bool[] left, bool?[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool?[left.Length];
            for (var i = 0; i < left.Length; i++)
                result[i] = left[i].Or(right[i]);
            return result;
        }

        /// <summary>
        /// Computes the logical OR operation between two boolean values.
        /// </summary>
        /// <param name="left">The left boolean value.</param>
        /// <param name="right">The right boolean value.</param>
        /// <returns>The result of the logical OR operation.</returns>
        public static bool Or(this bool left, bool right) => left | right;
    }
}