namespace NumeralSystems.Net.Utils
{
    /// <summary>
    /// Provides conversion methods for various data types.
    /// </summary>
    public static partial class Convert
    {
        /// <summary>
        /// Converts a boolean array to a decimal number.
        /// </summary>
        /// <param name="s">The boolean array to convert.</param>
        /// <returns>The decimal representation of the boolean array.</returns>
        public static decimal ToDecimal(this bool[] s){
            var bytes = ToByteArray(s);
            return ToDecimal(bytes);
        }

        /// <summary>
        /// Converts a boolean array into a decimal number.
        /// </summary>
        /// <param name="s">The boolean array to be converted.</param>
        /// <returns>The decimal representation of the boolean array.</returns>
        public static decimal ToDecimal(this byte[] s) => new (ToIntArray(s));

        /// <summary>
        /// Sets the value of a specified bit at the given index in a decimal number.
        /// </summary>
        /// <param name="b">The decimal number.</param>
        /// <param name="index">The index of the bit to set.</param>
        /// <param name="value">The value to set the bit to.</param>
        /// <returns>The updated decimal number with the specified bit set.</returns>
        public static decimal SetBoolAtIndex(this decimal b, uint index, bool value)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            bytes[byteIndex] = bytes[byteIndex].SetBoolAtIndex(bitIndex, value);
            return ToDecimal(bytes);
        }
    }
}