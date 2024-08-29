using System;

namespace NumeralSystems.Net.Utils
{
    /// <summary>
    /// The Convert class provides various conversion methods for different data types.
    /// </summary>
    public static partial class Convert
    {
        /// <summary>
        /// Converts a boolean array to a float value.
        /// </summary>
        /// <param name="s">The boolean array.</param>
        /// <returns>The corresponding float value.</returns>
        public static float ToFloat(this bool[] s)
        {
            var bytes = ToByteArray(s);
            return BitConverter.ToSingle(bytes);
        }

        /// <summary>
        /// Sets the value of a specified bit at the given index in a float.
        /// </summary>
        /// <param name="b">The float value.</param>
        /// <param name="index">The index of the bit to set.</param>
        /// <param name="value">The value to set the bit to.</param>
        /// <returns>The updated float value with the specified bit set.</returns>
        public static float SetBoolAtIndex(this float b, uint index, bool value)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            bytes[byteIndex] = bytes[byteIndex].SetBoolAtIndex(bitIndex, value);
            return BitConverter.ToSingle(bytes);
        }
    }
}