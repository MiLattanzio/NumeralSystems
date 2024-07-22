using System.Collections.Generic;
using NumeralSystems.Net.Type.Incomplete;

namespace NumeralSystems.Net.Interface
{
    public interface IIncompleteValue<TValue, TType> where TValue : INumeralValue<TType>
    {
        public bool?[] Binary { get; }
        public int Permutations { get; }
        public bool IsComplete { get; }
        public TValue this[int value] { get; }
        public IEnumerable<TValue> Enumerable { get; }
        public IncompleteByte[] ByteArray { get; }
        public IncompleteByte[] ToByteArray();
        public string ToString(string missingSeparator = "*");
    }
}