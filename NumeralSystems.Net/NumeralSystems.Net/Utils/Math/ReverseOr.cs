using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NumeralSystems.Net.Utils
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal partial class Math
    {
        /// <summary>
        /// Performs the logical OR operation between two nullable boolean values.
        /// </summary>
        /// <param name="left">The left nullable boolean value.</param>
        /// <param name="right">The right nullable boolean value.</param>
        /// <returns>
        /// If both <paramref name="left"/> and <paramref name="right"/> are null, returns null.
        /// If <paramref name="left"/> is false and <paramref name="right"/> is null, throws an <see cref="InvalidOperationException"/>.
        /// If <paramref name="left"/> is true and <paramref name="right"/> is null, returns true.
        /// If <paramref name="left"/> is null and <paramref name="right"/> is false, returns null.
        /// If <paramref name="left"/> is null and <paramref name="right"/> is true, throws an <see cref="InvalidOperationException"/>.
        /// If <paramref name="left"/> is false and <paramref name="right"/> is false, returns false.
        /// If <paramref name="left"/> is false and <paramref name="right"/> is true, throws an <see cref="InvalidOperationException"/>.
        /// If <paramref name="left"/> is true and <paramref name="right"/> is false, returns true.
        /// If <paramref name="left"/> is true and <paramref name="right"/> is true, returns null.
        /// </returns>
        public static bool? ReverseOr(this bool? left, bool? right) => (left, right) switch
        {
            (null, null) => null,
            (false, null) => throw new InvalidOperationException(),
            (true, null) => true,
            (null, false) => null,
            (null, true) => throw new InvalidOperationException(),
            (false, false) => false,
            (false, true) => throw new InvalidOperationException(),
            (true, false) => true,
            (true, true) => null
        };

        /// Performs the logical OR operation between two nullable boolean values.
        /// </summary>
        /// <param name="left">The left nullable boolean value.</param>
        /// <param name="right">The right nullable boolean value.</param>
        /// <returns>
        /// If both <paramref name="left"/> and <paramref name="right"/> are null, returns null.
        /// If <paramref name="left"/> is false and <paramref name="right"/> is null, throws an <see cref="InvalidOperationException"/>.
        /// If <paramref name="left"/> is true and <paramref name="right"/> is null, returns true.
        /// If <paramref name="left"/> is null and <paramref name="right"/> is false, returns null.
        /// If <paramref name="left"/> is null and <paramref name="right"/> is true, throws an <see cref="InvalidOperationException"/>.
        /// If <paramref name="left"/> is false and <paramref name="right"/> is false, returns false.
        /// If <paramref name="left"/> is false and <paramref name="right"/> is true, throws an <see cref="InvalidOperationException"/>.
        /// If <paramref name="left"/> is true and <paramref name="right"/> is false, returns true.
        /// If <paramref name="left"/> is true and <paramref name="right"/> is true, returns null.
        /// </returns>
        public static bool?[] ReverseOr(this bool[] left, bool[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("left and right must be the same length");

            var result = new bool?[left.Length];
            
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = ReverseOr(left[i], right[i]);
            }

            return result;
        }

        /// <summary>
        /// Performs the logical OR operation between two nullable boolean values.
        /// </summary>
        /// <param name="left">The left nullable boolean value.</param>
        /// <param name="right">The right nullable boolean value.</param>
        /// <returns>
        /// If both <paramref name="left"/> and <paramref name="right"/> are null, returns null.
        /// If <paramref name="left"/> is false and <paramref name="right"/> is null, throws an <see cref="InvalidOperationException"/>.
        /// If <paramref name="left"/> is true and <paramref name="right"/> is null, returns true.
        /// If <paramref name="left"/> is null and <paramref name="right"/> is false, returns null.
        /// If <paramref name="left"/> is null and <paramref name="right"/> is true, throws an <see cref="InvalidOperationException"/>.
        /// If <paramref name="left"/> is false and <paramref name="right"/> is false, returns false.
        /// If <paramref name="left"/> is false and <paramref name="right"/> is true, throws an <see cref="InvalidOperationException"/>.
        /// If <paramref name="left"/> is true and <paramref name="right"/> is false, returns true.
        /// If <paramref name="left"/> is true and <paramref name="right"/> is true, returns null.
        /// </returns>
        public static bool?[] ReverseOr(this bool[] left, bool?[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("left and right must be the same length");

            var result = new bool?[left.Length];
            
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = ReverseOr(left[i], right[i]);
            }

            return result;
        }

        /// Reverses the elements of the `left` boolean array and performs a bitwise OR operation with the corresponding elements of the `right` boolean array.
        /// Returns a new boolean array with the result of the operation.
        /// @param left The boolean array to be reversed and combined with `right`. Must have the same length as `right`.
        /// @param right The boolean array to be combined with the reversed `left`. Must have the same length as `left`.
        /// @return A new boolean array with the result of the bitwise OR operation between the reversed `left` and `right` arrays.
        /// @throws ArgumentException if `left` and `right` arrays have different lengths.
        /// /
        public static bool?[] ReverseOr(this bool?[] left, bool[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("left and right must be the same length");

            var result = new bool?[left.Length];
            
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = ReverseOr(left[i], right[i]);
            }

            return result;
        }

        /// <summary>
        /// Performs a bitwise OR operation on two arrays of nullable booleans, element-wise,
        /// and returns a new array with the result. The input arrays must have the same length.
        /// </summary>
        /// <param name="left">The first array of nullable booleans.</param>
        /// <param name="right">The second array of nullable booleans.</param>
        /// <returns>
        /// A new array of nullable booleans where each element is the result of applying the
        /// bitwise OR operation to the corresponding elements in the input arrays.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the lengths of the <paramref name="left"/> and <paramref name="right"/> arrays are different.
        /// </exception>
        public static bool?[] ReverseOr(this bool?[] left, bool?[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("left and right must be the same length");

            var result = new bool?[left.Length];
            
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = ReverseOr(left[i], right[i]);
            }

            return result;
        }
        
        public static bool CanReverseOr(this bool? left, bool? right) => (left, right) switch 
        {
            (false, null) => false,
            (null, true) => false,
            (false, true) => false,
            _ => true
        };

        /// Determines if each element in the left array of nullable booleans can be logically ORed with the corresponding element in the right array of nullable booleans.
        /// @param left An array of nullable booleans.
        /// @param right An array of nullable booleans with the same length as the left array.
        /// @return True if each element in the left array can be ORed with the corresponding element in the right array, false otherwise.
        /// /
        public static bool CanReverseOr(this bool?[] left, bool?[] right)
        {
            if (left.Length != right.Length) return false;
            return !left.Where((t, i) => !CanReverseOr(t, right[i])).Any();
        }
        
        public static bool CanReverseOr(this bool[] left, bool?[] right)
        {
            if (left.Length != right.Length) return false;
            return !left.Where((t, i) => !CanReverseOr(t, right[i])).Any();
        }

        /// <summary>
        /// Performs a logical OR operation between two nullable boolean values and returns the result.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>
        /// A nullable boolean value representing the result of the logical OR operation.
        /// - If both <paramref name="left"/> and <paramref name="right"/> are null, returns null.
        /// - If <paramref name="left"/> is false and <paramref name="right"/> is null, throws an <see cref="InvalidOperationException"/>.
        /// - If <paramref name="left"/> is true and <paramref name="right"/> is null, returns true.
        /// - If <paramref name="left"/> is null and <paramref name="right"/> is false, returns null.
        /// - If <paramref name="left"/> is null and <paramref name="right"/> is true, throws an <see cref="InvalidOperationException"/>.
        /// - If <paramref name="left"/> is false and <paramref name="right"/> is false, returns false.
        /// - If <paramref name="left"/> is false and <paramref name="right"/> is true, throws an <see cref="InvalidOperationException"/>.
        /// - If <paramref name="left"/> is true and <paramref name="right"/> is false, returns true.
        /// - If <paramref name="left"/> is true and <paramref name="right"/> is true, returns null.
        /// </returns>
        public static bool CanReverseOr(this bool[] left, bool[] right)
        {
            if (left.Length != right.Length) return false;
            return !left.Where((t, i) => !CanReverseOr(t, right[i])).Any();
        }

        /// <summary>
        /// Checks if each element in the left array can be reversed ORed with the corresponding element in the right array.
        /// If the lengths of the arrays differ, the method returns false.
        /// </summary>
        /// <param name="left">The array of nullable booleans representing the left side of the operation.</param>
        /// <param name="right">The array of booleans representing the right side of the operation.</param>
        /// <returns>True if every element in the left array can be reversed ORed with the corresponding element in the right array, false otherwise.</returns>
        public static bool CanReverseOr(this bool?[] left, bool[] right)
        {
            if (left.Length != right.Length) return false;
            return !left.Where((t, i) => !CanReverseOr(t, right[i])).Any();
        }
    }
}