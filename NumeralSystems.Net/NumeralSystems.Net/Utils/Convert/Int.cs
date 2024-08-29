using System;
using System.Linq;

namespace NumeralSystems.Net.Utils
{
    /// <summary>
    /// The Convert class provides static methods for converting different data types.
    /// </summary>
    public static partial class Convert
    {
        /// <summary>
        /// Converts a boolean array to an integer.
        /// </summary>
        /// <param name="s">The boolean array to convert.</param>
        /// <returns>The integer value obtained from the boolean array.</returns>
        public static int ToInt(this bool[] s) {
            if (null == s)
                s = Enumerable.Repeat(false, sizeof(int)).ToArray();
            else
                s = s.Length switch
                {
                    > sizeof(int) * 8 => s[0..(sizeof(int)*8)],
                    < sizeof(int) * 8 => Enumerable.Repeat(false, (sizeof(int)*8) - s.Length).Concat(s).ToArray(),
                    _ => s
                };
            int b = 0;
            foreach (var t in s.Reverse())
            {
                b <<= 1;
                if (t) b |= 1;
            }
            return b;
        }

        /// <summary>
        /// Converts a boolean array to an integer.
        /// </summary>
        /// <param name="s">The boolean array to convert.</param>
        /// <returns>The resulting integer.</returns>
        public static int ToInt(this byte[] s) => BitConverter.ToInt32(s, 0);

        /// <summary>
        /// Sets the value of a specified bit at the given index in a boolean array and returns the updated boolean array as an integer.
        /// </summary>
        /// <param name="b">The original integer value representing a boolean array.</param>
        /// <param name="index">The index of the bit to set.</param>
        /// <param name="value">The value to set the bit to.</param>
        /// <returns>The updated boolean array as an integer.</returns>
        public static int SetBoolAtIndex(this int b, uint index, bool value)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            bytes[byteIndex] = bytes[byteIndex].SetBoolAtIndex(bitIndex, value);
            return bytes.Select(x => x.ToBoolArray()).SelectMany(x =>x).ToArray().ToInt();
        }
    }
}