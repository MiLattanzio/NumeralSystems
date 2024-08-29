using System;

namespace NumeralSystems.Net.Utils
{
    public static partial class Convert
    {
        /// <summary>
        /// Converts a binary array to a double value.
        /// </summary>
        /// <param name="s">The binary array to convert.</param>
        /// <returns>The converted double value.</returns>
        public static double ToDouble(this bool[] s){
            var bytes = ToByteArray(s);
            return BitConverter.ToDouble(bytes);
        }

        /// <summary>
        /// Sets the value of a specified bit at the given index in a 64-bit double-precision floating-point number.
        /// </summary>
        /// <param name="b">The double value.</param>
        /// <param name="index">The index of the bit to set.</param>
        /// <param name="value">The value to set the bit to.</param>
        /// <returns>The updated double value with the specified bit set.</returns>
        public static double SetBoolAtIndex(this double b, uint index, bool value)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            bytes[byteIndex] = bytes[byteIndex].SetBoolAtIndex(bitIndex, value);
            return BitConverter.ToDouble(bytes);
        }
    }
}