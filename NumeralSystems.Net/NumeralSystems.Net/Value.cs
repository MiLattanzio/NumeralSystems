using System;
using System.Collections.Generic;
using System.Linq;
using NumeralSystems.Net.Utils;
using BigInteger = System.Numerics.BigInteger;

namespace NumeralSystems.Net
{
    /// <summary>
    /// Represents a numerical value with a specified base and a list of indices corresponding to the digits in that base.
    /// </summary>
    public class Value
    {
        /// <summary>
        /// Represents a value in a specified numeral system with a given base and indices.
        /// </summary>
        public Value(List<int> indices, int baseValue)
        {
            if (indices is null) throw new ArgumentNullException(nameof(indices));
            if (baseValue < 2) throw new ArgumentOutOfRangeException(nameof(baseValue), "Base must be at least 2.");
            Indices = indices.AsReadOnly();
            if (!Indices.All(x => x >= 0 && x < baseValue))
                throw new ArgumentOutOfRangeException(nameof(indices),
                    $"All indices must be within the range [0,{baseValue - 1}].");
            Base = baseValue;
        }

        /// <summary>
        /// Represents a read-only list of indices corresponding to a number in a specified numeral system base.
        /// </summary>
        /// <remarks>
        /// Each index represents a digit in a numeral system, where the system's base is defined by the <see cref="Base"/> property.
        /// All indices must be within the range [0, Base-1].
        /// </remarks>
        /// <value>
        /// A read-only list of integers representing the indices of a number in the specified numeral system base.
        /// </value>
        public IReadOnlyList<int> Indices { get; }

        /// <summary>
        /// Gets the base value used in the numeral representation of the indices.
        /// </summary>
        /// <remarks>
        /// The base value determines the range of valid indices. For a given base,
        /// the indices should all be in the range from 0 to base-1.
        /// </remarks>
        public int Base { get; }

        /// <summary>
        /// Creates a Value object from a string representation using a specified set of base indices.
        /// </summary>
        /// <param name="value">The string representation from which to create the Value. If the string is null, an empty Value object will be created.</param>
        /// <param name="baseIndices">A set of valid characters that define the base indices. Each character in the input string is matched against this set to form numerical indices.</param>
        /// <returns>A Value object representing the parsed input string with numerical indices based on the provided base indices set.</returns>
        [Obsolete(
            "HashSet<string> does not define numeric symbol order. Use FromString(string, NumeralAlphabet, string) instead.")]
        public static Value FromString(string value, HashSet<string> baseIndices)
        {
            if (baseIndices == null) throw new ArgumentNullException(nameof(baseIndices));
            var alphabet = new NumeralAlphabet(baseIndices.OrderBy(symbol => symbol, StringComparer.Ordinal));
            return FromString(value, alphabet);
        }

        /// <summary>
        /// Creates a value by decoding symbols with an ordered immutable alphabet.
        /// </summary>
        public static Value FromString(
            string value,
            NumeralAlphabet alphabet,
            string separator = "")
        {
            if (alphabet == null) throw new ArgumentNullException(nameof(alphabet));
            if (value == null) return new Value(new List<int>(), alphabet.Count);
            if (!alphabet.TryReadDigits(
                    value,
                    0,
                    value.Length,
                    separator ?? throw new ArgumentNullException(nameof(separator)),
                    out var indices,
                    out var errorPosition,
                    out var reason))
                throw new FormatException(
                    $"Invalid numeral at position {errorPosition}. Reason: {reason}.");
            return new Value(indices, alphabet.Count);
        }

        /// <summary>
        /// Creates a new <see cref="Value"/> instance from the given string and an optional fit parameter.
        /// </summary>
        /// <param name="value">The string representation to convert into a <see cref="Value"/>.</param>
        /// <param name="fit">A boolean indicating whether to fit the value within the smallest possible base.</param>
        /// <returns>A <see cref="Value"/> instance that represents the given string.</returns>
        public static Value FromString(string value, bool fit = false)
        {
            var indices = value.ToCharArray().Select(x => (int)x).ToList();
            var identity = fit ? indices.Max() + 1 : char.MaxValue;
            return new Value(indices, identity);
        }

        /// <summary>
        /// Creates a non-negative value from an arbitrary-precision integer.
        /// </summary>
        /// <param name="value">The non-negative integer to represent.</param>
        /// <param name="baseValue">The base used by the resulting digits.</param>
        /// <returns>A value containing the digits of <paramref name="value"/>.</returns>
        public static Value FromBigInteger(BigInteger value, int baseValue = 10)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Value cannot represent a negative integer.");

            return new Value(PositionalNotation.ToDigits(value, baseValue), baseValue);
        }

        /// <summary>
        /// Returns the arbitrary-precision integer represented by the current digits.
        /// </summary>
        public BigInteger ToBigInteger() => PositionalNotation.FromDigits(Indices, Base);

        /// <summary>
        /// Formats the stored digits with an ordered immutable alphabet.
        /// </summary>
        public string ToString(NumeralAlphabet alphabet, string separator = "")
        {
            if (alphabet == null) throw new ArgumentNullException(nameof(alphabet));
            if (separator == null) throw new ArgumentNullException(nameof(separator));
            if (alphabet.Count != Base)
                throw new ArgumentOutOfRangeException(
                    nameof(alphabet),
                    "Alphabet size must equal the value's base.");
            return string.Join(separator, Indices.Select(index => alphabet[index]));
        }


        /// <summary>
        /// Converts the current numeral value to a representation in a specified base.
        /// </summary>
        /// <param name="baseValue">The base to which the numeral value will be converted. It must be at least 2.</param>
        /// <param name="removeFirstZeros">Indicates whether leading zeros should be removed from the result. Default is false.</param>
        /// <returns>A new <see cref="Value"/> instance representing the number in the specified base.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="baseValue"/> is less than 2.</exception>
        public Value ToBase(int baseValue, bool removeFirstZeros = false)
        {
            var result = PositionalNotation.ToDigits(ToBigInteger(), baseValue);
            if (!removeFirstZeros)
            {
                var leadingZeros = Indices.TakeWhile(x => x == 0).Count();
                if (leadingZeros == Indices.Count) leadingZeros = System.Math.Max(0, leadingZeros - 1);
                result = Enumerable.Repeat(0, leadingZeros).Concat(result).ToList();
            }

            return new Value(result, baseValue);
        }

    }
}
