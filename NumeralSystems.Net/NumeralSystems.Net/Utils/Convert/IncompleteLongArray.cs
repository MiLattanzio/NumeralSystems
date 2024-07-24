using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NumeralSystems.Net.Type.Incomplete;

namespace NumeralSystems.Net.Utils
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static partial class Convert
    {
        public static IncompleteLong[] ToIncompleteInt64Array(this IEnumerable<IncompleteByte> s) => ToIncompleteInt64Array(new IncompleteByteArray() {
            Binary = s.Select(x => x.Binary).SelectMany(x => x).ToArray(),
        });
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