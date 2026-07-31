using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Globalization;
using NumeralSystems.Net.Utils;

namespace NumeralSystems.Net
{
    /// <summary>
    /// Represents an ordered, immutable, prefix-free numeral alphabet.
    /// </summary>
    public sealed class NumeralAlphabet : IReadOnlyList<string>, IEquatable<NumeralAlphabet>
    {
        private readonly string[] _symbols;
        private readonly IReadOnlyList<string> _readOnlySymbols;
        private readonly Dictionary<string, int> _indices;
        private readonly Dictionary<char, string[]> _symbolsByFirstCharacter;
        private readonly int? _fixedSymbolLength;

        /// <summary>
        /// Initializes an ordered alphabet in which each position is the numeric
        /// value of its symbol.
        /// </summary>
        public NumeralAlphabet(IEnumerable<string> symbols)
        {
            if (symbols == null) throw new ArgumentNullException(nameof(symbols));
            _symbols = symbols.ToArray();
            if (_symbols.Length < 2)
                throw new ArgumentException("A numeral alphabet requires at least two symbols.", nameof(symbols));
            if (_symbols.Any(string.IsNullOrEmpty))
                throw new ArgumentException("Numeral symbols cannot be null or empty.", nameof(symbols));

            _indices = new Dictionary<string, int>(StringComparer.Ordinal);
            for (var index = 0; index < _symbols.Length; index++)
            {
                if (!_indices.TryAdd(_symbols[index], index))
                    throw new ArgumentException(
                        $"The symbol '{_symbols[index]}' occurs more than once.",
                        nameof(symbols));
            }

            var orderedSymbols = _symbols.OrderBy(symbol => symbol, StringComparer.Ordinal).ToArray();
            for (var index = 0; index < orderedSymbols.Length - 1; index++)
            {
                if (orderedSymbols[index + 1].StartsWith(
                        orderedSymbols[index],
                        StringComparison.Ordinal))
                    throw new ArgumentException(
                        $"The symbols '{orderedSymbols[index]}' and '{orderedSymbols[index + 1]}' " +
                        "have an ambiguous prefix.",
                        nameof(symbols));
            }

            _readOnlySymbols = new ReadOnlyCollection<string>(_symbols);
            _fixedSymbolLength = _symbols.All(symbol => symbol.Length == _symbols[0].Length)
                ? _symbols[0].Length
                : (int?)null;
            _symbolsByFirstCharacter = _symbols
                .GroupBy(symbol => symbol[0])
                .ToDictionary(
                    group => group.Key,
                    group => group.OrderByDescending(symbol => symbol.Length).ToArray());
        }

        /// <summary>
        /// Gets the binary alphabet <c>01</c>.
        /// </summary>
        public static NumeralAlphabet Base2 { get; } = FromCharacters("01");

        /// <summary>
        /// Gets the octal alphabet <c>01234567</c>.
        /// </summary>
        public static NumeralAlphabet Base8 { get; } = FromCharacters("01234567");

        /// <summary>
        /// Gets the decimal alphabet <c>0123456789</c>.
        /// </summary>
        public static NumeralAlphabet Base10 { get; } = FromCharacters("0123456789");

        /// <summary>
        /// Gets the uppercase hexadecimal alphabet.
        /// </summary>
        public static NumeralAlphabet Base16 { get; } = FromCharacters("0123456789ABCDEF");

        /// <summary>
        /// Gets the Crockford-style Base32 alphabet without I, L, O, or U.
        /// </summary>
        public static NumeralAlphabet Base32 { get; } =
            FromCharacters("0123456789ABCDEFGHJKMNPQRSTVWXYZ");

        /// <summary>
        /// Gets the uppercase alphanumeric Base36 alphabet.
        /// </summary>
        public static NumeralAlphabet Base36 { get; } =
            FromCharacters("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ");

        /// <summary>
        /// Gets the Bitcoin Base58 alphabet.
        /// </summary>
        public static NumeralAlphabet Base58 { get; } =
            FromCharacters("123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz");

        /// <summary>
        /// Gets a digits-first Base62 alphabet.
        /// </summary>
        public static NumeralAlphabet Base62 { get; } =
            FromCharacters("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");

        /// <summary>
        /// Gets the RFC 4648 Base64 alphabet without padding.
        /// </summary>
        public static NumeralAlphabet Base64 { get; } =
            FromCharacters("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/");

        /// <summary>
        /// Gets the bases for which a named predefined alphabet is available.
        /// </summary>
        public static IReadOnlyList<int> PredefinedBases { get; } =
            new ReadOnlyCollection<int>(new[] { 2, 8, 10, 16, 32, 36, 58, 62, 64 });

        /// <summary>
        /// Gets the number of symbols and therefore the positional base.
        /// </summary>
        public int Count => _symbols.Length;

        /// <summary>
        /// Gets the symbol whose numeric value is <paramref name="index"/>.
        /// </summary>
        public string this[int index] => _symbols[index];

        /// <summary>
        /// Gets the immutable ordered symbol collection.
        /// </summary>
        public IReadOnlyList<string> Symbols => _readOnlySymbols;

        /// <summary>
        /// Returns the predefined alphabet for a supported base.
        /// </summary>
        public static NumeralAlphabet ForBase(int baseValue) => baseValue switch
        {
            2 => Base2,
            8 => Base8,
            10 => Base10,
            16 => Base16,
            32 => Base32,
            36 => Base36,
            58 => Base58,
            62 => Base62,
            64 => Base64,
            _ => throw new ArgumentOutOfRangeException(
                nameof(baseValue),
                $"No predefined alphabet is available for base {baseValue}.")
        };

        /// <summary>
        /// Tries to obtain a predefined alphabet.
        /// </summary>
        public static bool TryForBase(int baseValue, out NumeralAlphabet alphabet)
        {
            switch (baseValue)
            {
                case 2: alphabet = Base2; return true;
                case 8: alphabet = Base8; return true;
                case 10: alphabet = Base10; return true;
                case 16: alphabet = Base16; return true;
                case 32: alphabet = Base32; return true;
                case 36: alphabet = Base36; return true;
                case 58: alphabet = Base58; return true;
                case 62: alphabet = Base62; return true;
                case 64: alphabet = Base64; return true;
                default:
                    alphabet = null;
                    return false;
            }
        }

        /// <summary>
        /// Returns a predefined alphabet when available, otherwise creates a
        /// deterministic, fixed-width decimal-symbol alphabet.
        /// </summary>
        public static NumeralAlphabet CreateDefault(int baseValue)
        {
            if (baseValue < 2)
                throw new ArgumentOutOfRangeException(
                    nameof(baseValue),
                    "A positional numeral alphabet requires a base of at least 2.");
            if (TryForBase(baseValue, out var predefined)) return predefined;

            var width = (baseValue - 1).ToString(CultureInfo.InvariantCulture).Length;
            var format = "D" + width.ToString(CultureInfo.InvariantCulture);
            return new NumeralAlphabet(
                Enumerable.Range(0, baseValue)
                    .Select(index => index.ToString(format, CultureInfo.InvariantCulture)));
        }

        /// <summary>
        /// Returns the numeric index of a symbol, or -1 when it is absent.
        /// </summary>
        public int IndexOf(string symbol) =>
            symbol != null && _indices.TryGetValue(symbol, out var index) ? index : -1;

        /// <summary>
        /// Tests whether the alphabet contains a symbol using ordinal comparison.
        /// </summary>
        public bool Contains(string symbol) => IndexOf(symbol) >= 0;

        /// <summary>
        /// Validates that formatting tokens cannot be mistaken for digits or for
        /// one another.
        /// </summary>
        public void ValidateFormat(
            string separator,
            string negativeSign,
            string numberDecimalSeparator)
        {
            if (separator == null) throw new ArgumentNullException(nameof(separator));
            if (string.IsNullOrEmpty(negativeSign))
                throw new ArgumentException("The negative sign cannot be null or empty.", nameof(negativeSign));
            if (string.IsNullOrEmpty(numberDecimalSeparator))
                throw new ArgumentException(
                    "The decimal separator cannot be null or empty.",
                    nameof(numberDecimalSeparator));

            var tokens = new[]
                {
                    (Name: "digit separator", Value: separator),
                    (Name: "negative sign", Value: negativeSign),
                    (Name: "decimal separator", Value: numberDecimalSeparator)
                }
                .Where(token => token.Value.Length > 0)
                .ToArray();

            foreach (var symbol in _symbols)
            {
                foreach (var token in tokens)
                {
                    if (symbol.Contains(token.Value, StringComparison.Ordinal) ||
                        token.Value.Contains(symbol, StringComparison.Ordinal))
                        throw new ArgumentException(
                            $"The {token.Name} '{token.Value}' conflicts with digit symbol '{symbol}'.");
                }
            }

            for (var left = 0; left < tokens.Length; left++)
            {
                for (var right = left + 1; right < tokens.Length; right++)
                {
                    if (tokens[left].Value.Contains(tokens[right].Value, StringComparison.Ordinal) ||
                        tokens[right].Value.Contains(tokens[left].Value, StringComparison.Ordinal))
                        throw new ArgumentException(
                            $"The {tokens[left].Name} '{tokens[left].Value}' conflicts with the " +
                            $"{tokens[right].Name} '{tokens[right].Value}'.");
                }
            }
        }

        /// <summary>
        /// Encodes a signed arbitrary-precision integer.
        /// </summary>
        public string Encode(
            BigInteger value,
            string separator = "",
            string negativeSign = "-")
        {
            ValidateIntegerFormat(separator, negativeSign);
            var negative = value < BigInteger.Zero;
            var magnitude = BigInteger.Abs(value);
            var digits = PositionalNotation.ToDigits(magnitude, Count);
            var encoded = string.Join(separator, digits.Select(index => _symbols[index]));
            return negative ? negativeSign + encoded : encoded;
        }

        /// <summary>
        /// Decodes a signed arbitrary-precision integer.
        /// </summary>
        /// <exception cref="FormatException">The text does not contain a valid numeral.</exception>
        public BigInteger Decode(
            string value,
            string separator = "",
            string negativeSign = "-")
        {
            if (!TryDecode(value, out var result, out var errorPosition, separator, negativeSign))
                throw new FormatException($"Invalid numeral at position {errorPosition}.");
            return result;
        }

        /// <summary>
        /// Tries to decode a signed arbitrary-precision integer.
        /// </summary>
        public bool TryDecode(
            string value,
            out BigInteger result,
            string separator = "",
            string negativeSign = "-") =>
            TryDecode(value, out result, out _, separator, negativeSign);

        /// <summary>
        /// Tries to decode a signed arbitrary-precision integer and reports the
        /// UTF-16 position at which decoding failed.
        /// </summary>
        public bool TryDecode(
            string value,
            out BigInteger result,
            out int errorPosition,
            string separator = "",
            string negativeSign = "-")
        {
            ValidateIntegerFormat(separator, negativeSign);
            result = BigInteger.Zero;
            errorPosition = 0;
            if (string.IsNullOrEmpty(value)) return false;

            var position = 0;
            var negative = false;
            if (value.StartsWith(negativeSign, StringComparison.Ordinal))
            {
                negative = true;
                position = negativeSign.Length;
                if (position == value.Length)
                {
                    errorPosition = position;
                    return false;
                }
            }

            if (!TryReadDigits(
                    value,
                    position,
                    value.Length,
                    separator,
                    out var digits,
                    out errorPosition,
                    out _))
                return false;

            result = PositionalNotation.FromDigits(digits, Count);
            if (negative) result = BigInteger.Negate(result);
            errorPosition = -1;
            return true;
        }

#if NET8_0_OR_GREATER
        /// <summary>Encodes into a caller-provided character span.</summary>
        public bool TryEncode(
            BigInteger value,
            Span<char> destination,
            out int charactersWritten,
            string separator = "",
            string negativeSign = "-")
        {
            var encoded = Encode(value, separator, negativeSign);
            charactersWritten = 0;
            if (encoded.Length > destination.Length) return false;
            encoded.AsSpan().CopyTo(destination);
            charactersWritten = encoded.Length;
            return true;
        }

        /// <summary>Decodes a character span on modern .NET targets.</summary>
        public BigInteger Decode(
            ReadOnlySpan<char> value,
            string separator = "",
            string negativeSign = "-") =>
            Decode(value.ToString(), separator, negativeSign);

        /// <summary>Attempts to decode a character span on modern .NET targets.</summary>
        public bool TryDecode(
            ReadOnlySpan<char> value,
            out BigInteger result,
            string separator = "",
            string negativeSign = "-") =>
            TryDecode(value.ToString(), out result, separator, negativeSign);
#endif

        internal bool TryReadDigits(
            string value,
            int start,
            int end,
            string separator,
            out List<int> digits,
            out int errorPosition,
            out ParseErrorReason reason)
        {
            digits = new List<int>();
            errorPosition = start;
            reason = ParseErrorReason.UnknownSymbol;
            if (start >= end)
            {
                reason = ParseErrorReason.MissingDigit;
                return false;
            }

            var position = start;
            var expectDigit = true;
            while (position < end)
            {
                if (!string.IsNullOrEmpty(separator) &&
                    Matches(value, position, separator, end))
                {
                    if (expectDigit)
                    {
                        errorPosition = position;
                        reason = ParseErrorReason.UnexpectedSeparator;
                        return false;
                    }

                    expectDigit = true;
                    position += separator.Length;
                    continue;
                }

                if (!expectDigit && !string.IsNullOrEmpty(separator))
                {
                    errorPosition = position;
                    reason = ParseErrorReason.MissingSeparator;
                    return false;
                }

                if (!TryMatch(value, position, end, out var index, out var length))
                {
                    errorPosition = position;
                    reason = ParseErrorReason.UnknownSymbol;
                    return false;
                }

                digits.Add(index);
                position += length;
                expectDigit = false;
            }

            if (expectDigit)
            {
                errorPosition = end;
                reason = ParseErrorReason.MissingDigit;
                return false;
            }

            errorPosition = -1;
            reason = ParseErrorReason.None;
            return true;
        }

        internal bool TryMatch(string value, int position, int end, out int index, out int length)
        {
            index = -1;
            length = 0;
            if (value == null || position < 0 || position >= end || end > value.Length)
                return false;

            if (_fixedSymbolLength.HasValue)
            {
                length = _fixedSymbolLength.Value;
                if (position + length > end)
                {
                    length = 0;
                    return false;
                }

                var symbol = value.Substring(position, length);
                if (_indices.TryGetValue(symbol, out index)) return true;
                index = -1;
                length = 0;
                return false;
            }

            if (!_symbolsByFirstCharacter.TryGetValue(value[position], out var candidates))
                return false;

            foreach (var symbol in candidates)
            {
                if (!Matches(value, position, symbol, end)) continue;
                index = _indices[symbol];
                length = symbol.Length;
                return true;
            }

            return false;
        }

        internal static bool Matches(string value, int position, string token, int end)
        {
            if (position < 0 || token == null || position + token.Length > end) return false;
            return string.CompareOrdinal(value, position, token, 0, token.Length) == 0;
        }

        /// <inheritdoc />
        public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)_symbols).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        public bool Equals(NumeralAlphabet other) =>
            other != null && _symbols.SequenceEqual(other._symbols, StringComparer.Ordinal);

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(obj as NumeralAlphabet);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                foreach (var symbol in _symbols)
                    hash = hash * 31 + StringComparer.Ordinal.GetHashCode(symbol);
                return hash;
            }
        }

        /// <inheritdoc />
        public override string ToString() => string.Concat(_symbols);

        private static NumeralAlphabet FromCharacters(string characters) =>
            new NumeralAlphabet(characters.Select(character => character.ToString()));

        private void ValidateIntegerFormat(string separator, string negativeSign)
        {
            if (separator == null) throw new ArgumentNullException(nameof(separator));
            if (string.IsNullOrEmpty(negativeSign))
                throw new ArgumentException("The negative sign cannot be null or empty.", nameof(negativeSign));

            foreach (var symbol in _symbols)
            {
                if (!string.IsNullOrEmpty(separator) &&
                    (symbol.Contains(separator, StringComparison.Ordinal) ||
                     separator.Contains(symbol, StringComparison.Ordinal)))
                    throw new ArgumentException(
                        $"The digit separator '{separator}' conflicts with digit symbol '{symbol}'.",
                        nameof(separator));
                if (symbol.Contains(negativeSign, StringComparison.Ordinal) ||
                    negativeSign.Contains(symbol, StringComparison.Ordinal))
                    throw new ArgumentException(
                        $"The negative sign '{negativeSign}' conflicts with digit symbol '{symbol}'.",
                        nameof(negativeSign));
            }

            if (!string.IsNullOrEmpty(separator) &&
                (separator.Contains(negativeSign, StringComparison.Ordinal) ||
                 negativeSign.Contains(separator, StringComparison.Ordinal)))
                throw new ArgumentException(
                    "The digit separator conflicts with the negative sign.",
                    nameof(separator));
        }
    }
}
