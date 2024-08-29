using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NumeralSystems.Net.Utils
{
    /// <summary>
    /// The Convert class provides methods for converting data types.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static partial class Convert
    {
        /// <summary>
        /// Converts a byte array to an integer array.
        /// </summary>
        /// <param name="s">The byte array to convert.</param>
        /// <returns>The integer array created from the byte array.</returns>
        public static int[] ToIntArray(this byte[] s)
        {
            var result = new List<int>();
            for (var i = 0; i < s.Length; i += 4)
                result.Add(BitConverter.ToInt32(s, i));
            return result.ToArray();
        }
        
        
    }
}