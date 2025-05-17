using System;
using System.Diagnostics.CodeAnalysis;
using Polecola.Primitive;

namespace NumeralSystems.Net.Utils
{
    /// <summary>
    /// Provides mathematical operations and utilities.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal static partial class Math
    {
        /// Performs a bitwise AND operation on two boolean arrays.
        /// @param a The first boolean array.
        /// @param b The second boolean array.
        /// @return The result of the bitwise AND operation as a boolean array.
        /// @throws ArgumentException if the lengths of the input arrays are not equal.
        /// /
        public static bool[] And(this bool[] a, bool[] b)
        {
            if (a.Length != b.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool[a.Length];
            for (var i = 0; i < a.Length; i++)
                result[i] = a[i] && b[i];
            return result;
        }

        /// <summary>
        /// Performs a bitwise AND operation between two boolean arrays.
        /// </summary>
        /// <param name="a">The first boolean array.</param>
        /// <param name="b">The second boolean array.</param>
        /// <returns>A new boolean array containing the result of the bitwise AND operation.</returns>
        public static bool?[] And(this bool?[] a, bool?[] b)
        {
            if (a.Length != b.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool?[a.Length];
            for (var i = 0; i < a.Length; i++)
                if (null == a[i] || null == b[i])
                    result[i] = null;
                else
                    result[i] = a[i].Value && b[i].Value;
            return result;
        }

        /// Performs the logical AND operation on two boolean arrays and returns the result.
        /// Throws an ArgumentException if the arrays are not of equal length.
        /// @param a - The first boolean array.
        /// @param b - The second boolean array.
        /// @return The result of the logical AND operation as a boolean array.
        /// /
        public static bool?[] And(this bool[] a, bool?[] b)
        {
            if (a.Length != b.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool?[a.Length];
            for (var i = 0; i < a.Length; i++)
                if (null == b[i])
                    result[i] = null;
                else
                    result[i] = a[i] && b[i].Value;
            return result;
        }

        /// <summary>
        /// Performs a bitwise AND operation on two boolean arrays.
        /// </summary>
        /// <param name="a">The first boolean array.</param>
        /// <param name="b">The second boolean array.</param>
        /// <returns>A boolean array that represents the result of the AND operation between the two input arrays.</returns>
        public static bool?[] And(this bool?[] a, bool[] b)
        {
            if (a.Length != b.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool?[a.Length];
            for (var i = 0; i < a.Length; i++)
                if (null == a[i])
                    result[i] = null;
                else
                    result[i] = a[i].Value && b[i];
            return result;
        }

        public static decimal And(this decimal a, decimal b) =>
            And(a.ToBoolArray(), b.ToBoolArray()).ToDecimal();

        /// <summary>
        /// Performs a bitwise AND operation on two bool arrays and returns the result.
        /// </summary>
        /// <param name="a">The first bool array.</param>
        /// <param name="b">The second bool array.</param>
        /// <returns>A bool array containing the result of the bitwise AND operation.</returns>
        public static double And(this double a, double b) =>
            And(a.ToBoolArray(), b.ToBoolArray()).ToDouble();

        /// <summary>
        /// Performs a logical AND operation on two boolean arrays element by element.
        /// </summary>
        /// <param name="a">The first boolean array.</param>
        /// <param name="b">The second boolean array.</param>
        /// <returns>A new boolean array resulting from the logical AND operation.</returns>
        /// <exception cref="ArgumentException">Thrown when the lengths of the input arrays are not equal.</exception>
        public static float And(this float a, float b) =>
            And(a.ToBoolArray(), b.ToBoolArray()).ToFloat();

    }
}