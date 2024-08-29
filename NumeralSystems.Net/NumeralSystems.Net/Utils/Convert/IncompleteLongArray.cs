using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NumeralSystems.Net.Type.Incomplete;

namespace NumeralSystems.Net.Utils
{
    /// <summary>
    /// The Convert class provides methods for converting between different numeral systems and data types.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static partial class Convert
    {
        public static IncompleteLong[] ToIncompleteInt64Array(this IEnumerable<IncompleteByte> s) => ToIncompleteInt64Array(new IncompleteByteArray() {
            Binary = s.Select(x => x.Binary).SelectMany(x => x).ToArray(),
        });

        /// <summary>
        /// Converts an enumerable of <see cref="IncompleteByte"/> to an array of <see cref="IncompleteLong"/>.
        /// </summary>
        /// <param name="s">The enumerable of <see cref="IncompleteByte"/> to convert.</param>
        /// <returns>An array of <see cref="IncompleteLong"/> representing the converted values.</returns>
        public static IncompleteLong[] ToIncompleteInt64Array(this IncompleteByteArray s)
        {
            var chars = new List<IncompleteLong>();
            for (var i = 0; i < (s.Binary.Length / (8 * sizeof(long))); i++)
            {
                chars.Add(new()
                {
                    Binary = s.Binary.Skip(i * (8 * sizeof(long))).Take((8 * sizeof(long))).ToArray()
                });
            }
            return chars.ToArray();
        }
    }
}