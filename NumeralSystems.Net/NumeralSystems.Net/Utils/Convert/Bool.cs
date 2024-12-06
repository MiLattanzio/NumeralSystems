using System.Numerics;

namespace NumeralSystems.Net.Utils
{
    public static partial class Convert
    {
        /// <summary>
        /// Gets the boolean value at the specified index in a byte.
        /// </summary>
        /// <param name="b">The byte value.</param>
        /// <param name="index">The index of the bit.</param>
        /// <returns>The boolean value at the specified index.</returns>
        public static bool GetBoolAtIndex(this byte b, uint index)
        {
            return (b & (1 << (int)index)) != 0;
        }

        /// <summary>
        /// Gets the boolean value at the specified index in a short.
        /// </summary>
        /// <param name="b">The short value.</param>
        /// <param name="index">The index of the bit.</param>
        /// <returns>The boolean value at the specified index.</returns>
        public static bool GetBoolAtIndex(this short b, uint index)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            return bytes[byteIndex].GetBoolAtIndex(bitIndex);
        }

        /// <summary>
        /// Retrieves the boolean value at the specified index from the given byte.
        /// </summary>
        /// <param name="b">The byte from which to retrieve the boolean value.</param>
        /// <param name="index">The index of the boolean value to retrieve.</param>
        /// <returns>The boolean value at the specified index.</returns>
        public static bool GetBoolAtIndex(this ushort b, uint index)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            return bytes[byteIndex].GetBoolAtIndex(bitIndex);
        }

        /// <summary>
        /// Retrieves the boolean value at a specific index of a byte.
        /// </summary>
        /// <param name="b">The byte value.</param>
        /// <param name="index">The index of the bit.</param>
        /// <returns>The boolean value at the specified index.</returns>
        public static bool GetBoolAtIndex(this int b, uint index)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            return bytes[byteIndex].GetBoolAtIndex(bitIndex);
        }

        /// <summary>
        /// Retrieves the Boolean value at the specified index in a byte.
        /// </summary>
        /// <param name="b">The byte value.</param>
        /// <param name="index">The index.</param>
        /// <returns>The Boolean value at the specified index.</returns>
        public static bool GetBoolAtIndex(this uint b, uint index)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            return bytes[byteIndex].GetBoolAtIndex(bitIndex);
        }


        /// <summary>
        /// Retrieves the bool value at the specified index from a byte.
        /// </summary>
        /// <param name="b">The byte value.</param>
        /// <param name="index">The index of the bool value to retrieve.</param>
        /// <returns>The bool value at the specified index.</returns>
        public static bool GetBoolAtIndex(this float b, uint index)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            return bytes[byteIndex].GetBoolAtIndex(bitIndex);
        }

        /// <summary>
        /// Retrieves the boolean value at the specified index from a byte value.
        /// </summary>
        /// <param name="b">The byte value.</param>
        /// <param name="index">The index of the boolean value to retrieve.</param>
        /// <returns>The boolean value at the specified index.</returns>
        public static bool GetBoolAtIndex(this long b, uint index)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            return bytes[byteIndex].GetBoolAtIndex(bitIndex);
        }

        /// <summary>
        /// Gets the value of the bit at the specified index in a byte.
        /// </summary>
        /// <param name="b">The byte value.</param>
        /// <param name="index">The index of the target bit (0-7).</param>
        /// <returns>The boolean value of the specified bit.</returns>
        public static bool GetBoolAtIndex(this ulong b, uint index)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            return bytes[byteIndex].GetBoolAtIndex(bitIndex);
        }

        /// <summary>
        /// Retrieves the boolean value at the specified index in the given byte.
        /// </summary>
        /// <param name="b">The byte value from which to retrieve the boolean.</param>
        /// <param name="index">The index of the boolean to retrieve, ranging from 0 to 7.</param>
        /// <returns>
        /// <c>true</c> if the boolean at the specified index is 1;
        /// otherwise, <c>false</c> if the boolean at the specified index is 0.
        /// </returns>
        public static bool GetBoolAtIndex(this double b, uint index)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            return bytes[byteIndex].GetBoolAtIndex(bitIndex);
        }

        /// <summary>
        /// Retrieves the value of a boolean at the specified index from a byte.
        /// </summary>
        /// <param name="b">The byte from which to retrieve the boolean value.</param>
        /// <param name="index">The index of the boolean value to retrieve.</param>
        /// <returns>The boolean value at the specified index in the byte.</returns>
        public static bool GetBoolAtIndex(this decimal b, uint index)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            return bytes[byteIndex].GetBoolAtIndex(bitIndex);
        }

        /// <summary>
        /// Gets the boolean value at the specified index from a byte value.
        /// </summary>
        /// <param name="b">The byte value.</param>
        /// <param name="index">The index of the boolean value to retrieve.</param>
        /// <returns>The boolean value at the specified index.</returns>
        public static bool GetBoolAtIndex(this char b, uint index)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            return bytes[byteIndex].GetBoolAtIndex(bitIndex);
        }

        /// <summary>
        /// Retrieves the boolean value at the specified index from a byte.
        /// </summary>
        /// <param name="b">The byte value.</param>
        /// <param name="index">The index of the boolean value to retrieve. The index is zero-based.</param>
        /// <returns>The boolean value at the specified index.</returns>
        public static bool GetBoolAtIndex(this BigInteger b, uint index)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            return bytes[(int)byteIndex].GetBoolAtIndex(bitIndex);
        }
        
        
    }
}