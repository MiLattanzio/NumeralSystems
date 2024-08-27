using System.Collections.Generic;
using NumeralSystems.Net.Type.Incomplete;

namespace NumeralSystems.Net.Interface
{
    /// <summary>
    /// Represents a value that is not complete in its binary representation.
    /// </summary>
    /// <typeparam name="TValue">The incomplete source type</typeparam>
    /// <typeparam name="TType">The complete source type representation</typeparam>
    /// <typeparam name="TIndexer">The type used to index the permutations</typeparam>
    public interface IIncompleteValue<out TValue, TType, TIndexer> where TValue : INumeralValue<TType> where TIndexer: struct
    {
        /// <summary>
        /// Incomplete binary representation of the value.
        /// </summary>
        public bool?[] Binary { get; }
        /// <summary>
        /// Permutations count of the incomplete value.
        /// </summary>
        public TIndexer Permutations { get; }
        /// <summary>
        /// If the value is complete.
        /// </summary>
        public bool IsComplete { get; }
        /// <summary>
        /// Gets the complete value representation at the given index.
        /// </summary>
        /// <param name="value">Index used to fetch the nth representation of the value</param>
        public TValue this[TIndexer value] { get; }
        /// <summary>
        /// Enumerable of all possible values.
        /// </summary>
        public IEnumerable<TValue> Enumerable { get; }
        /// <summary>
        /// Incomplete value as incomplete byte array pointing to the memory address of the incomplete value.
        /// </summary>
        public IncompleteByte[] ByteArray { get; }
        /// <summary>
        /// Copy of the incomplete value as incomplete byte array.
        /// </summary>
        /// <returns>A copy of the incomplete byte array that represents the value</returns>
        public IncompleteByte[] ToByteArray();
        /// <summary>
        /// String representation of the incomplete value boolean array.
        /// </summary>
        /// <param name="missingSeparator">Used to represent the null value</param>
        /// <returns>A string representation for the incomplete value</returns>
        public string ToString(string missingSeparator = "*");
    }
}