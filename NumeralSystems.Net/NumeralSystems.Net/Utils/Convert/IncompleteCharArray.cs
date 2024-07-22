using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NumeralSystems.Net.Type.Incomplete;

namespace NumeralSystems.Net.Utils
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static partial class Convert
    {
        public static IncompleteChar[] ToIncompleteCharArray(this IEnumerable<IncompleteByte> s) => ToIncompleteCharArray(new IncompleteByteArray() {
            Binary = s.Select(x => x.Binary).SelectMany(x => x).ToArray(),
        });
        public static IncompleteChar[] ToIncompleteCharArray(this IncompleteByteArray s)
        {
            var chars = new List<IncompleteChar>();
            for (var i = 0; i < (s.Binary.Length / (8 * sizeof(char))); i++)
            {
                chars.Add(new()
                {
                    Binary = s.Binary.Skip(i * (8 * sizeof(char))).Take((8 * sizeof(char))).ToArray()
                });
            }
            return chars.ToArray();
        }
    }
}