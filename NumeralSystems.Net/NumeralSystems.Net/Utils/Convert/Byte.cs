using System.Linq;

namespace NumeralSystems.Net.Utils
{
    /// The Convert class provides utility methods for converting data types in the NumeralSystems.Net.Utils namespace.
    /// This class contains the following methods for converting binary data to a byte:
    /// - ToByte: Converts an array of boolean values representing binary data to a byte value.
    /// - SetBoolAtIndex: Sets the value of a specific bit at the given index in a byte value.
    /// The Convert class is used by other classes in the NumeralSystems.Net.Utils namespace, such as Byte.cs, IncompleteDecimal.cs, Float.cs, Char.cs, IncompleteFloat.cs, and IncompleteByte.cs.
    /// To use the Convert class, include the appropriate "using Convert = NumeralSystems.Net.Utils.Convert;" statement in your code file.
    /// @see Byte
    /// @see IncompleteDecimal
    /// @see Float
    /// @see Char
    /// @see IncompleteFloat
    /// @see IncompleteByte
    /// /
    public static partial class Convert
    {
        /// <summary>
        /// Converts an array of boolean values representing bits into a byte.
        /// </summary>
        /// <param name="s">An array of boolean values, where each value represents a bit.</param>
        /// <returns>A byte where the array of boolean values has been converted into its byte representation.</returns>
        public static byte ToByte(this bool[] s) {
            if (null == s)
                s = Enumerable.Repeat(false, sizeof(byte)).ToArray();
            else
                s = s.Length switch
                {
                    > sizeof(byte) * 8 => s[0..(sizeof(byte)*8)],
                    < sizeof(byte) * 8 => Enumerable.Repeat(false, (sizeof(byte)*8) - s.Length).Concat(s).ToArray(),
                    _ => s
                };
            byte b = 0;
            foreach (var t in s.Reverse())
            {
                b <<= 1;
                if (t) b |= 1;
            }
            return b;
        }

        /// <summary>
        /// Sets the value of a specified bit at the given index in a byte.
        /// </summary>
        /// <param name="b">The byte value.</param>
        /// <param name="index">The index of the bit to set.</param>
        /// <param name="value">The value to set the bit to.</param>
        /// <returns>The updated byte value with the specified bit set.</returns>
        public static byte SetBoolAtIndex(this byte b, uint index, bool value)
        {
            if (value) return (byte)(b | (1 << (int)index));
            return (byte)(b & ~(1 << (int)index));
        }
    }
}