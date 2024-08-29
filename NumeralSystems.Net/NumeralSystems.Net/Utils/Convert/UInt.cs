using System;
using System.Linq;

namespace NumeralSystems.Net.Utils
{
    public static partial class Convert
    {
        /// <summary>
        /// Converts a boolean array to an unsigned 32-bit integer.
        /// </summary>
        /// <param name="s">The boolean array to convert.</param>
        /// <returns>The converted unsigned 32-bit integer.</returns>
        public static uint ToUInt(this bool[] s)
        {
            const int bitsInUint = sizeof(uint) * 8;
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

            uint result = 0;
            foreach (var bit in s.Reverse())
            {
                result = (result << 1) | (bit ? 1u : 0u);
            }

            return result;
        }
        public static uint ToUInt(this byte[] s) => BitConverter.ToUInt32(s, 0);

        /// <summary>
        /// Sets the value of a specified bit at the given index in an unsigned 32-bit integer.
        /// </summary>
        /// <param name="b">The unsigned 32-bit integer value.</param>
        /// <param name="index">The index of the bit to set.</param>
        /// <param name="value">The value to set the bit to.</param>
        /// <returns>The updated unsigned 32-bit integer value with the specified bit set.</returns>
        public static uint SetBoolAtIndex(this uint b, uint index, bool value)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            bytes[byteIndex] = bytes[byteIndex].SetBoolAtIndex(bitIndex, value);
            return bytes.Select(x => x.ToBoolArray()).SelectMany(x => x).ToArray().ToUInt();
        }
    }
}