using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace NumeralSystems.Net.Type.Base
{
    public partial class String
    {
        public static string EncodeToBase(string s, int destinationBase, out int size)
        {
            if (destinationBase <= 0 || destinationBase > char.MaxValue)
                throw new System.ArgumentException("Destination base must be greater than 0 and less than or equal to " + char.MaxValue);
            var indices = ToIndicesOfBase(s, destinationBase).ToList();
            var len = indices.Max(x => x.Length);
            var adjustedIndices = indices.Select(x => x.Length < len ? Enumerable.Repeat(0U, len - x.Length).Concat(x).ToArray() : x).ToArray();
            size = len;
            return string.Concat(adjustedIndices.SelectMany(x => x.Select(y => (char)y)));
        }
        public static string DecodeFromBase(string s, int sourceBase, int size)
        {
            if (sourceBase <= 0 || sourceBase > char.MaxValue)
                throw new System.ArgumentException("Source base must be greater than 0 and less than or equal to " + char.MaxValue);
            var indices = s.Select(c => (uint)c).ToArray();
            var len = indices.Length / size;
            var adjustedIndices = Enumerable.Range(0, len).Select(x => indices.Skip(x * size).Take(size).ToArray()).ToArray();
            return FromIndicesOfBase(adjustedIndices, sourceBase);
        }
        public static IEnumerable<uint[]> ToIndicesOfBase(string s, int destinationBase) => s.Select(c => UInt.ToIndicesOfBase(c, destinationBase, out var _));
        public static string FromIndicesOfBase(IEnumerable<uint[]> s, int sourceBase) => string.Concat(s.Select(c => (char)UInt.FromIndicesOfBase(c, sourceBase, true)));
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