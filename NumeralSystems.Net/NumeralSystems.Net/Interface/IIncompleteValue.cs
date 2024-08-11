using System.Collections.Generic;
using NumeralSystems.Net.Type.Incomplete;

namespace NumeralSystems.Net.Interface
{
    public interface IIncompleteValue<TValue, TType, TIndexer> where TValue : INumeralValue<TType> where TIndexer: struct
    {
        public bool?[] Binary { get; }
        public TIndexer Permutations { get; }
        public bool IsComplete { get; }
        public TValue this[TIndexer value] { get; }
        public IEnumerable<TValue> Enumerable { get; }
        public IncompleteByte[] ByteArray { get; }
        public IncompleteByte[] ToByteArray();
        public string ToString(string missingSeparator = "*");
    }
}