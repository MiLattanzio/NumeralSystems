using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NumeralSystems.Net.Type.Incomplete;

namespace NumeralSystems.Net.Utils
{
    /// <summary>
    /// A utility class for converting data between different numeral systems and types.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static partial class Convert
    {
        /// <summary>
        /// Converts a collection of <see cref="IncompleteByte"/> objects to an array of <see cref="IncompleteInt"/> objects.
        /// </summary>
        /// <param name="s">The collection of <see cref="IncompleteByte"/> objects to convert.</param>
        /// <returns>An array of <see cref="IncompleteInt"/> objects.</returns>
        public static IncompleteInt[] ToIncompleteInt32Array(this IEnumerable<IncompleteByte> s) => ToIncompleteInt32Array(new IncompleteByteArray() {
            Binary = s.Select(x => x.Binary).SelectMany(x => x).ToArray(),
        });

        /// <summary>
        /// Converts an enumerable collection of IncompleteByte objects to an array of IncompleteInt objects.
        /// </summary>
        /// <param name="s">The enumerable collection of IncompleteByte objects.</param>
        /// <returns>An array of IncompleteInt objects.</returns>
        public static IncompleteInt[] ToIncompleteInt32Array(this IncompleteByteArray s)
        {
            var chars = new List<IncompleteInt>();
            for (var i = 0; i < (s.Binary.Length / (8 * sizeof(int))); i++)
            {
                chars.Add(new()
                {
                    Binary = s.Binary.Skip(i * (8 * sizeof(int))).Take((8 * sizeof(int))).ToArray()
                });
            }
            return chars.ToArray();
        }
    }
}