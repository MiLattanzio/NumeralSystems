using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NumeralSystems.Net.Encoding
{
    /// <summary>
    /// Extracts distinct text units in first-occurrence order. UTF-16 code units
    /// and Unicode scalar values are exposed as separate operations.
    /// </summary>
    public static class CharacterIdentity
    {
        /// <summary>Gets distinct UTF-16 code units in first-occurrence order.</summary>
        public static IReadOnlyList<char> GetUtf16CodeUnits(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            var seen = new HashSet<char>();
            var result = new List<char>();
            foreach (var codeUnit in value)
                if (seen.Add(codeUnit)) result.Add(codeUnit);
            return new ReadOnlyCollection<char>(result);
        }

#if NET8_0_OR_GREATER
        /// <summary>
        /// Gets distinct Unicode scalar values in first-occurrence order.
        /// Unpaired surrogates are rejected.
        /// </summary>
        public static IReadOnlyList<Rune> GetRunes(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            CharacterRadixTransform.GetSmallestBaseRunes(value);
            var seen = new HashSet<Rune>();
            var result = new List<Rune>();
            foreach (var rune in value.EnumerateRunes())
                if (seen.Add(rune)) result.Add(rune);
            return new ReadOnlyCollection<Rune>(result);
        }
#endif
    }
}
