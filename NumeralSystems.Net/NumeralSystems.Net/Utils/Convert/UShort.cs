using System;
using System.Linq;

namespace NumeralSystems.Net.Utils
{
    public static partial class Convert
    {
        /// <summary>
        /// Converts a boolean array to an unsigned short integer.
        /// </summary>
        /// <param name="s">The boolean array to convert.</param>
        /// <returns>The converted unsigned short integer.</returns>
        public static ushort ToUShort(this bool[] s)
        {
            const int bitsInUint = sizeof(short) * 8;
            if (s == null)
            {
                s = new bool[bitsInUint];
            }
            else if (s.Length > bitsInUint)
            {
                s = s.TakeLast(bitsInUint).ToArray();
            }
            else if (s.Length < bitsInUint)
            {
                s = Enumerable.Repeat(false, bitsInUint - s.Length).Concat(s).ToArray();
            }

            return s.Reverse().Aggregate<bool, ushort>(0, (current, bit) => (ushort)((current << 1) | (bit ? 1 : 0)));
        }

        /// <summary>
        /// Converts a boolean array to an unsigned short integer.
        /// </summary>
        /// <param name="s">The boolean array to convert.</param>
        /// <returns>The converted unsigned short integer.</returns>
        public static ushort ToUShort(this byte[] s) => BitConverter.ToUInt16(s, 0);

        /// <summary>
        /// Sets the value of a specified bit at the given index in a ushort.
        /// </summary>
        /// <param name="b">The ushort value.</param>
        /// <param name="index">The index of the bit to set.</param>
        /// <param name="value">The value to set the bit to.</param>
        /// <returns>The updated ushort value with the specified bit set.</returns>
        public static ushort SetBoolAtIndex(this ushort b, uint index, bool value)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            bytes[byteIndex] = bytes[byteIndex].SetBoolAtIndex(bitIndex, value);
            return bytes.Select(x => x.ToBoolArray()).SelectMany(x => x).ToArray().ToUShort();
        }
    }
}