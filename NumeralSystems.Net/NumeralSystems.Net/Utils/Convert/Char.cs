using System.Linq;

namespace NumeralSystems.Net.Utils
{
    /// <summary>
    /// Provides conversion utilities for various data types.
    /// </summary>
    public static partial class Convert
    {
        /// <summary>
        /// Converts a binary array to a character.
        /// </summary>
        /// <param name="s">The binary array to convert.</param>
        /// <returns>The character representation of the binary array.</returns>
        public static char ToChar(this bool[] s) {
            if (null == s)
                s = Enumerable.Repeat(false, sizeof(char)).ToArray();
            else
                s = s.Length switch
                {
                    > sizeof(char) * 8 => s[0..(sizeof(char)*8)],
                    < sizeof(char) * 8 => Enumerable.Repeat(false, (sizeof(char)*8) - s.Length).Concat(s).ToArray(),
                    _ => s
                };
            char b = (char)0;
            foreach (var t in s.Reverse())
            {
                b <<= 1;
                if (t) b |= (char)1;
            }
            return b;
        }

        /// Sets the value of the bit at the given index in a character.
        /// <param name="b">The character value.</param>
        /// <param name="index">The index of the bit to set.</param>
        /// <param name="value">The value to set the bit to.</param>
        /// <returns>The updated character value with the specified bit set.</returns>
        public static char SetBoolAtIndex(this char b, uint index, bool value)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            bytes[byteIndex] = bytes[byteIndex].SetBoolAtIndex(bitIndex, value);
            return bytes.Select(x => x.ToBoolArray()).SelectMany(x =>x).ToArray().ToChar();
        }
    }
}