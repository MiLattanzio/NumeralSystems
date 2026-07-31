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
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the destination base is outside [2, 65536].</exception>
        [System.Obsolete(
            "This is an experimental UTF-16 code-unit transform, not a standard base encoding. " +
            "Use CharacterRadixTransform.EncodeUtf16.")]
        public static string EncodeToBase(string s, int destinationBase, out int size)
        {
            return NumeralSystems.Net.Encoding.CharacterRadixTransform.EncodeUtf16(
                s,
                destinationBase,
                out size);
        }

        /// <summary>
        /// Decodes a string from a specified base.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="sourceBase">The base to decode from.</param>
        /// <param name="size">The size of the encoded string.</param>
        /// <returns>The decoded string.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the source base is outside [2, 65536].</exception>
        [System.Obsolete(
            "This is an experimental UTF-16 code-unit transform, not a standard base encoding. " +
            "Use CharacterRadixTransform.DecodeUtf16.")]
        public static string DecodeFromBase(string s, int sourceBase, int size)
        {
            return NumeralSystems.Net.Encoding.CharacterRadixTransform.DecodeUtf16(
                s,
                sourceBase,
                size);
        }

        /// <summary>
        /// Converts a string to its indices representation in a specified base.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <param name="destinationBase">The base to convert to.</param>
        /// <returns>An enumerable of uint arrays representing the indices.</returns>
        [System.Obsolete(
            "This is an experimental UTF-16 code-unit transform. " +
            "Use CharacterRadixTransform or Value.FromUtf16String.")]
        public static IEnumerable<uint[]> ToIndicesOfBase(string s, int destinationBase) => s.Select(c => UInt.ToIndicesOfBase(c, destinationBase, out var _));

        /// <summary>
        /// Converts indices representation in a specified base to a string.
        /// </summary>
        /// <param name="s">The indices to convert.</param>
        /// <param name="sourceBase">The base of the indices.</param>
        /// <returns>The decoded string.</returns>
        [System.Obsolete(
            "This is an experimental UTF-16 code-unit transform. " +
            "Use CharacterRadixTransform or Value.ToUtf16String.")]
        public static string FromIndicesOfBase(IEnumerable<uint[]> s, int sourceBase) => string.Concat(s.Select(c => (char)UInt.FromIndicesOfBase(c, sourceBase, true)));

        /// <summary>
        /// Gets the smallest base that can represent all characters in a string.
        /// </summary>
        /// <param name="s">The string to analyze.</param>
        /// <returns>
        /// The smallest base strictly greater than the maximum UTF-16 code-unit
        /// value, or 2 for an empty string.
        /// </returns>
        [System.Obsolete(
            "Use CharacterRadixTransform.GetSmallestBaseUtf16; the result is max digit + 1.")]
        public static int GetSmallestBase(string s)
        {
            return NumeralSystems.Net.Encoding.CharacterRadixTransform.GetSmallestBaseUtf16(s);
        }
    }
}
