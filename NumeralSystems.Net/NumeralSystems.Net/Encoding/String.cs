using System.Collections.Generic;
using System.Linq;

namespace NumeralSystems.Net.Encoding
{
    /// <summary>
    /// String encoding utilities.
    /// </summary>
    [System.Obsolete(
        "This legacy name extracts UTF-16 code units; it does not encode text. " +
        "Use CharacterIdentity.GetUtf16CodeUnits or CharacterIdentity.GetRunes.")]
    public class String
    {
        /// <summary>
        /// Extracts the distinct characters from the string ordered by count.
        /// </summary>
        /// <param name="value">Value to extract the identity from</param>
        /// <returns>The identity of the value</returns>
        /// <remarks>For the Numeral type usage</remarks>
        public IList<char> GetIdentity(string value)
        {
            return CharacterIdentity.GetUtf16CodeUnits(value).ToList();
        }
    }
}
