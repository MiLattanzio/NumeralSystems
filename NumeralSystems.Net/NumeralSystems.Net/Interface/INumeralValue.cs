
using NumeralSystems.Net.Utils;

namespace NumeralSystems.Net.Interface
{
    /// <summary>
    /// Represents a numeral value.
    /// </summary>
    /// <typeparam name="TValue">The native type of the value</typeparam>
    public interface INumeralValue<TValue>
    {
        /// <summary>
        /// The value of the numeral.
        /// </summary>
        public TValue Value { get; set; }
        /// <summary>
        /// The value of the numeral as a byte array.
        /// </summary>
        public byte[] Bytes { get; }
        /// <summary>
        /// The value of the numeral as a binary array.
        /// </summary>
        public bool[] Binary { get; }
        /// <summary>
        /// The length of the numeral type in bits.
        /// </summary>
        public int BitLength { get; }
        /// <summary>
        /// Gets the value of the numeral as a string.
        /// </summary>
        /// <param name="format">Formatter for the value</param>
        /// <returns>The value formatted with the given input</returns>
        public string ToString(string format);
        /// <summary>
        /// Get and set the value of the numeral at the given index.
        /// </summary>
        /// <param name="index">Index of the bit</param>
        public bool this[int index]
        {
            get;
            set;
        }
    }
    
}