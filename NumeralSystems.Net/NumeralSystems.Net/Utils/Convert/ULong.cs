using System;
using System.Linq;

namespace NumeralSystems.Net.Utils
{
    /// <summary>
    /// The Convert class provides conversion methods for various data types.
    /// </summary>
    public static partial class Convert
    {
        /// <summary>
        /// Converts a boolean array to an unsigned long integer.
        /// </summary>
        /// <param name="s">The boolean array.</param>
        /// <returns>The converted unsigned long integer.</returns>
        public static ulong ToULong(this bool[] s) {
            if (null == s)
                s = Enumerable.Repeat(false, sizeof(ulong)).ToArray();
            else
                s = s.Length switch
                {
                    > sizeof(ulong) * 8 => s[0..(sizeof(ulong)*8)],
                    < sizeof(ulong) * 8 => Enumerable.Repeat(false, (sizeof(ulong)*8) - s.Length).Concat(s).ToArray(),
                    _ => s
                };
            ulong b = 0;
            foreach (var t in s.Reverse())
            {
                b <<= 1;
                if (t) b |= 1;
            }
            return b;
        }

        /// <summary>
        /// Converts a boolean array to an unsigned long (ulong) value.
        /// </summary>
        /// <param name="s">The boolean array to convert.</param>
        /// <returns>The unsigned long value representing the boolean array.</returns>
        public static ulong ToULong(this byte[] s) => BitConverter.ToUInt64(s, 0);

        /// <summary>
        /// Sets the value of a specified bit at the given index in a ulong value.
        /// </summary>
        /// <param name="b">The ulong value.</param>
        /// <param name="index">The index of the bit to set.</param>
        /// <param name="value">The value to set the bit to.</param>
        /// <returns>The updated ulong value with the specified bit set.</returns>
        public static ulong SetBoolAtIndex(this ulong b, uint index, bool value)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            bytes[byteIndex] = bytes[byteIndex].SetBoolAtIndex(bitIndex, value);
            return bytes.Select(x => x.ToBoolArray()).SelectMany(x =>x).ToArray().ToULong();
        }
    }
}