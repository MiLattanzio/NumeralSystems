using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NumeralSystems.Net.Utils
{
    /// The Math class provides various mathematical operations.
    /// /
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal static partial class Math
    {
        /// <summary>
        /// Returns the negation of the specified nullable boolean value.
        /// </summary>
        /// <param name="value">The nullable boolean value.</param>
        /// <returns>The negated boolean value or null if the input value is null.</returns>
        public static bool? Not(this bool? value) => value switch
        {
            null => null,
            false => true,
            true => false
        };
        public static bool Not(this bool value) => !value;

        /// <summary>
        /// Performs a logical NOT operation on a boolean value.
        /// </summary>
        /// <param name="value">The boolean value to perform the NOT operation on.</param>
        /// <returns>The result of the logical NOT operation.</returns>
        public static bool[] Not(this bool[] value) => value.Select(Not).ToArray();

        /// <summary>
        /// Performs a bitwise NOT operation on a boolean value.
        /// </summary>
        /// <param name="value">The boolean value to perform the NOT operation on.</param>
        /// <returns>The result of the NOT operation.</returns>
        public static bool?[] Not(this bool?[] value) => value.Select(Not).ToArray();
    }
}