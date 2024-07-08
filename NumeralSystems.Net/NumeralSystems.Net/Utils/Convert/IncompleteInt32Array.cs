using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NumeralSystems.Net.Type.Incomplete;

namespace NumeralSystems.Net.Utils
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static partial class Convert
    {
        public static IncompleteInt32[] ToIncompleteInt32Array(this IEnumerable<IncompleteByte> s) => ToIncompleteInt32Array(new IncompleteByteArray() {
            Binary = s.Select(x => x.Binary).SelectMany(x => x).ToArray(),
        });
        public static IncompleteInt32[] ToIncompleteInt32Array(this IncompleteByteArray s)
        {
            var chars = new List<IncompleteInt32>();
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