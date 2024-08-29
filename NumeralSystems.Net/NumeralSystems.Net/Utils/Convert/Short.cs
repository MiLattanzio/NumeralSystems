using System;
using System.Linq;

namespace NumeralSystems.Net.Utils
{
    public static partial class Convert
    {
        /// <summary>
        /// Converts a boolean array to a short value.
        /// </summary>
        /// <param name="s">The boolean array to convert. If null, a default array of length sizeof(short)*8 will be created.</param>
        /// <returns>The converted short value.</returns>
        public static short ToShort(this bool[] s)
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

            return s.Reverse().Aggregate<bool, short>(0, (current, bit) => (short)((current << 1) | (bit ? 1 : 0)));
        }

        /// <summary>
        /// Converts a boolean array to a short value.
        /// </summary>
        /// <param name="s">The boolean array to convert. If null, a default array of length sizeof(short)*8 will be created.</param>
        /// <returns>The converted short value.</returns>
        public static short ToShort(this byte[] s) => BitConverter.ToInt16(s, 0);

        /// <summary>
        /// Sets the value of a specified bit at the given index in a short.
        /// </summary>
        /// <param name="b">The short value.</param>
        /// <param name="index">The index of the bit to set.</param>
        /// <param name="value">The value to set the bit to.</param>
        /// <returns>The updated short value with the specified bit set.</returns>
        public static short SetBoolAtIndex(this short b, uint index, bool value)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            bytes[byteIndex] = bytes[byteIndex].SetBoolAtIndex(bitIndex, value);
            return bytes.Select(x => x.ToBoolArray()).SelectMany(x => x).ToArray().ToShort();
        }
    }
}