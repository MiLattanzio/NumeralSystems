using System.Linq;
using System.Numerics;

namespace NumeralSystems.Net.Utils
{
    /// <summary>
    /// A utility class that provides methods for converting different types to boolean arrays.
    /// </summary>
    public static partial class Convert
    {
        /// <summary>
        /// Converts a byte to a boolean array representation.
        /// </summary>
        /// <param name="b">The byte to convert.</param>
        /// <returns>The boolean array representation of the byte.</returns>
        public static bool[] ToBoolArray(this byte b) {
            var s = new bool[8];
            for (var i = 0; i < s.Length; i++)
            {
                s[i] = (b & (1 << i)) != 0;
            }
            return s;
        }

        /// <summary>
        /// Converts a byte to a boolean array representation.
        /// </summary>
        /// <param name="b">The byte to convert.</param>
        /// <returns>The boolean array representation of the byte.</returns>
        public static bool[] ToBoolArray(this BigInteger c) => c.ToByteArray().Select(ToBoolArray).SelectMany(x => x).ToArray();

        /// *Parameters:**
        public static bool[] ToBoolArray(this char c) => ToByteArray(c).Select(ToBoolArray).SelectMany(x => x).ToArray();

        /// <summary>
        /// Converts a byte to a boolean array.
        /// </summary>
        /// <param name="b">The byte to convert.</param>
        /// <returns>The boolean array representing the byte.</returns>
        public static bool[] ToBoolArray(this short b) => ToByteArray(b).Select(ToBoolArray).SelectMany(x => x).ToArray();
        public static bool[] ToBoolArray(this ushort b) => ToByteArray(b).Select(ToBoolArray).SelectMany(x => x).ToArray();

        /// <summary>
        /// Converts a uint value to a bool array representation.
        /// </summary>
        /// <param name="b">The uint value to convert.</param>
        /// <returns>The bool array representation of the uint value.</returns>
        public static bool[] ToBoolArray(this uint b) => ToByteArray(b).Select(ToBoolArray).SelectMany(x => x).ToArray();
        public static bool[] ToBoolArray(this int b) => ToByteArray(b).Select(ToBoolArray).SelectMany(x => x).ToArray();

        /// <summary>
        /// Converts a byte to a boolean array.
        /// </summary>
        /// <param name="b">The byte value to convert</param>
        /// <returns>The boolean array representation of the byte</returns>
        public static bool[] ToBoolArray(this float b) => ToByteArray(b).Select(ToBoolArray).SelectMany(x => x).ToArray();

        /// <summary>
        /// Converts a byte value to a boolean array.
        /// </summary>
        /// <param name="b">The byte value to convert.</param>
        /// <returns>A boolean array representing the byte value.</returns>
        public static bool[] ToBoolArray(this double b) => ToByteArray(b).Select(ToBoolArray).SelectMany(x => x).ToArray();

        /// Converts a byte value to a boolean array.
        /// @param b The byte value to convert.
        /// @return The boolean array representing the binary representation of the byte value.
        /// /
        public static bool[] ToBoolArray(this long b) => ToByteArray(b).Select(ToBoolArray).SelectMany(x => x).ToArray();

        /// Converts a byte to a boolean array.
        /// @param b The byte to be converted.
        /// @return The boolean array representation of the byte. The array has 8 elements, where each element represents a bit of the byte.
        /// /
        public static bool[] ToBoolArray(this decimal b) => ToByteArray(b).Select(ToBoolArray).SelectMany(x => x).ToArray();

        /// <summary>
        /// Converts an unsigned long integer to a boolean array representing its binary representation.
        /// </summary>
        /// <param name="b">The unsigned long integer to convert.</param>
        /// <returns>A boolean array representing the binary representation of the unsigned long integer.</returns>
        public static bool[] ToBoolArray(this ulong b) => ToByteArray(b).Select(ToBoolArray).SelectMany(x => x).ToArray();
    }
}