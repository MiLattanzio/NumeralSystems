using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NumeralSystems.Net.Utils
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static partial class Convert
    {
        /// <summary>
        /// Converts a boolean array to a byte array.
        /// </summary>
        /// <param name="s">The boolean array to convert.</param>
        /// <returns>The byte array representation of the boolean array.</returns>
        public static byte[] ToByteArray(this bool[] s) {
            if (null == s)
                s = Enumerable.Repeat(false, 8).ToArray();
            else
            {
                System.Math.DivRem(s.Length, 8, out var rem);
                if (rem > 0)
                    s = s.Concat(Enumerable.Repeat(false, 8 - rem)).ToArray();
            }
            var b = new byte[s.Length / 8];
            for (var i = 0; i < b.Length; i++)
                b[i] = ToByte(s.Skip(i * 8).Take(8).ToArray());
            return b;
        }

        /// <summary>
        /// Converts a boolean array to a byte array.
        /// </summary>
        /// <param name="s">The boolean array to convert.</param>
        /// <returns>The resulting byte array.</returns>
        public static byte[] ToByteArray(this uint s) => BitConverter.GetBytes(s);

        /// <summary>
        /// Converts a boolean array to a byte array.
        /// </summary>
        /// <param name="s">The boolean array to convert.</param>
        /// <returns>The resulting byte array.</returns>
        public static byte[] ToByteArray(this int s) => BitConverter.GetBytes(s);

        /// <summary>
        /// Converts a boolean array to a byte array.
        /// </summary>
        /// <param name="s">The boolean array to convert.</param>
        /// <returns>A byte array.</returns>
        public static byte[] ToByteArray(this IEnumerable<int> s) => s.Select(ToByteArray).SelectMany(x => x).ToArray();

        /// <summary>
        /// Converts a boolean array to a byte array.
        /// </summary>
        /// <param name="s">The boolean array to be converted.</param>
        /// <returns>The byte array representation of the boolean array.</returns>
        public static byte[] ToByteArray(this char c) => BitConverter.GetBytes(c);

        /// <summary>
        /// Converts the specified boolean array to a byte array.
        /// </summary>
        /// <param name="s">The boolean array to convert.</param>
        /// <returns>The byte array.</returns>
        public static byte[] ToByteArray(this short s) => BitConverter.GetBytes(s);

        /// <summary>
        /// Converts a boolean array to a byte array.
        /// </summary>
        /// <param name="s">The boolean array to convert.</param>
        /// <returns>The converted byte array.</returns>
        public static byte[] ToByteArray(this ushort s) => BitConverter.GetBytes(s);

        /// <summary>
        /// Converts the given value to a byte array.
        /// </summary>
        /// <param name="s">The value to convert.</param>
        /// <returns>
        /// The byte array representation of the given value.
        /// </returns>
        public static byte[] ToByteArray(this long s) => BitConverter.GetBytes(s);

        /// <summary>
        /// Converts a boolean array to a byte array.
        /// </summary>
        /// <param name="s">The boolean array to convert.</param>
        /// <returns>A byte array representing the boolean values.</returns>
        public static byte[] ToByteArray(this double s) => BitConverter.GetBytes(s);

        /// <summary>
        /// Converts a boolean array to a byte array.
        /// </summary>
        /// <param name="s">The boolean array to convert</param>
        /// <returns>The byte array representation of the boolean array</returns>
        public static byte[] ToByteArray(this float s) => BitConverter.GetBytes(s);

        /// <summary>
        /// Converts a boolean array to a byte array.
        /// </summary>
        /// <param name="s">The boolean array to be converted.</param>
        /// <returns>The byte array representation of the boolean array.</returns
        public static byte[] ToByteArray(this decimal s) => decimal.GetBits(s).Select(ToByteArray).SelectMany(x => x).ToArray();

        /// *Parameters:**
        public static byte[] ToByteArray(this ulong s) => BitConverter.GetBytes(s);
    }
}