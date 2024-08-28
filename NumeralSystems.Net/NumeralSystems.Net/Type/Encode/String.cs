using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace NumeralSystems.Net.Type.Base
{
    public partial class String
    {
        /// <summary>
        /// Encodes a string to a specified base.
        /// </summary>
        /// <param name="s">The string to encode.</param>
        /// <param name="destinationBase">The base to encode to.</param>
        /// <param name="size">The size of the encoded string.</param>
        /// <returns>The encoded string.</returns>
        /// <exception cref="System.ArgumentException">Thrown when the destination base is less than or equal to 0 or greater than char.MaxValue.</exception>
        public static string EncodeToBase(string s, int destinationBase, out int size)
        {
            if (destinationBase <= 0 || destinationBase > char.MaxValue)
                throw new System.ArgumentException("Destination base must be greater than 0 and less than or equal to " + char.MaxValue);
            var indices = ToIndicesOfBase(s, destinationBase).ToList();
            var len = indices.Max(x => x.Length);
            var adjustedIndices = indices.Select(x => x.Length < len ? System.Linq.Enumerable.Repeat(0U, len - x.Length).Concat(x).ToArray() : x).ToArray();
            size = len;
            return string.Concat(adjustedIndices.SelectMany(x => x.Select(y => (char)y)));
        }

        /// <summary>
        /// Decodes a string from a specified base.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="sourceBase">The base to decode from.</param>
        /// <param name="size">The size of the encoded string.</param>
        /// <returns>The decoded string.</returns>
        /// <exception cref="System.ArgumentException">Thrown when the source base is less than or equal to 0 or greater than char.MaxValue.</exception>
        public static string DecodeFromBase(string s, int sourceBase, int size)
        {
            if (sourceBase <= 0 || sourceBase > char.MaxValue)
                throw new System.ArgumentException("Source base must be greater than 0 and less than or equal to " + char.MaxValue);
            var indices = s.Select(c => (uint)c).ToArray();
            var len = indices.Length / size;
            var adjustedIndices = System.Linq.Enumerable.Range(0, len).Select(x => indices.Skip(x * size).Take(size).ToArray()).ToArray();
            return FromIndicesOfBase(adjustedIndices, sourceBase);
        }

        /// <summary>
        /// Converts a string to its indices representation in a specified base.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <param name="destinationBase">The base to convert to.</param>
        /// <returns>An enumerable of uint arrays representing the indices.</returns>
        public static IEnumerable<uint[]> ToIndicesOfBase(string s, int destinationBase) => s.Select(c => UInt.ToIndicesOfBase(c, destinationBase, out var _));

        /// <summary>
        /// Converts indices representation in a specified base to a string.
        /// </summary>
        /// <param name="s">The indices to convert.</param>
        /// <param name="sourceBase">The base of the indices.</param>
        /// <returns>The decoded string.</returns>
        public static string FromIndicesOfBase(IEnumerable<uint[]> s, int sourceBase) => string.Concat(s.Select(c => (char)UInt.FromIndicesOfBase(c, sourceBase, true)));

        /// <summary>
        /// Gets the smallest base that can represent all characters in a string.
        /// </summary>
        /// <param name="s">The string to analyze.</param>
        /// <returns>The smallest base that can represent all characters in the string.</returns>
        public static int GetSmallestBase(string s)
        {
            var smallestBase = 0;
            foreach (var c in s)
            {
                if (c > smallestBase)
                {
                    smallestBase = c;
                }
            }
            return smallestBase;
        }
    }
}