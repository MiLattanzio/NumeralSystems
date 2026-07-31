using System;
using BigInt = System.Numerics.BigInteger;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NumeralSystems.Net.Type.Base;
using NumeralSystems.Net.Utils;
using Math = System.Math;

#nullable enable annotations

namespace NumeralSystems.Net
{
    /// Represents a numeral in a specific numeral system.
    public class Numeral : IFormattable
#if NET8_0_OR_GREATER
        , ISpanFormattable
#endif
    {
        private readonly bool _positive = true;
        private readonly RationalValue? _exactValueOverride;

        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// Gets a value indicating whether the number is positive or negative.
        /// </summary>
        public bool Positive => _positive;

        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// Represents a numeral in a specific numeral system.
        /// </summary>
        public NumeralSystem Base { get; }

        /// <summary>
        /// Gets an immutable exact rational snapshot of the current digit representation.
        /// </summary>
        public RationalValue ExactValue => _exactValueOverride ?? RationalValue.FromDigits(
            _integralIndices,
            _fractionalIndices,
            !Positive,
            Base.Size);

        /// <summary>
        /// Fractional indices are the indices of the fractional part of the number
        /// </summary>
        private readonly List<int> _fractionalIndices = new();

        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// Gets a copy of the fractional indices for the numeral.
        /// </summary>
        /// <remarks>
        /// The fractional indices represent the positions of the fractional part of the numeral in the identity list.
        /// </remarks>
        public List<int> FractionalIndices => new List<int>(_fractionalIndices);

        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// Retrieves the fractional part of a number represented as a collection of strings in a specific numeral system.
        /// </summary>
        /// <param name="identity">The collection of strings representing the number's identity in the numeral system.</param>
        /// <returns>A list of strings representing the fractional part of the number.</returns>
        public List<string> GetFractionalStrings(IList<string> identity)
        {
            if (identity.Count < Base.Size)
                throw new ArgumentOutOfRangeException(nameof(identity),
                    "Identity must be at least the size of the base");
            return FractionalIndices.Select(identity.ElementAt).ToList();
        }

        /// <summary>
        /// Gets fractional digit symbols from an ordered alphabet.
        /// </summary>
        public List<string> GetFractionalStrings(NumeralAlphabet alphabet)
        {
            EnsureAlphabet(alphabet);
            return FractionalIndices.Select(index => alphabet[index]).ToList();
        }

        /// <summary>
        /// Gets the fractional part of the identity as a string, using the provided separator.
        /// </summary>
        /// <param name="identity">The list of strings representing the identity.</param>
        /// <param name="separator">The string used to separate each fractional part.</param>
        /// <returns>A string representing the fractional part of the identity, separated by the provided separator. If there is no fractional part, returns the first element of the identity list.</returns>
        public string GetFractionalString(IList<string> identity, string separator)
        {
            var result = string.Join(separator, GetFractionalStrings(identity));
            return string.IsNullOrEmpty(result) ? identity[0] : result;
        }

        /// <summary>
        /// Gets the formatted fractional digit sequence.
        /// </summary>
        public string GetFractionalString(NumeralAlphabet alphabet, string separator = "")
        {
            if (separator == null) throw new ArgumentNullException(nameof(separator));
            var result = string.Join(separator, GetFractionalStrings(alphabet));
            return string.IsNullOrEmpty(result) ? alphabet[0] : result;
        }


        /// <summary>
        /// Represents the list of integral indices of a Numeral object.
        /// </summary>
        private readonly List<int> _integralIndices = new();

        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// Gets a copy of the integral indices representing a numeral system's number.
        /// </summary>
        /// <remarks>
        /// The integral indices represent the positions of the digits in a number within a specific numeral system.
        /// The indices are stored as a list of integers.
        /// </remarks>
        public List<int> IntegralIndices => new List<int>(_integralIndices);

        /// <summary>
        /// Gets the integral digits of a numeral as a list of string representations.
        /// </summary>
        /// <param name="identity">The list of string representations of the identity of the numeral system (e.g., "0", "1", "2", ...) must have at least the same size as the base.</param>
        /// <returns>A list of string representations of the integral digits of the numeral.</returns>
        public List<string> GetIntegralStrings(IList<string> identity)
        {
            if (identity.Count < Base.Size)
                throw new ArgumentOutOfRangeException(nameof(identity),
                    "Identity must be at least the size of the base");
            return IntegralIndices.Select(identity.ElementAt).ToList();
        }

        /// <summary>
        /// Gets integral digit symbols from an ordered alphabet.
        /// </summary>
        public List<string> GetIntegralStrings(NumeralAlphabet alphabet)
        {
            EnsureAlphabet(alphabet);
            return IntegralIndices.Select(index => alphabet[index]).ToList();
        }

        /// Returns the integral part of a number represented in a given numeral system as a string.
        /// If the integral part is empty, it returns the first element of the identity.
        /// @param identity The identity of the numeral system represented as a list of strings.
        /// @param separator The separator to be used between the integral digits.
        /// @return The integral part of the number as a string.
        /// @throws ArgumentOutOfRangeException If the size of the identity is less than the size of the numeral system's base.
        /// /
        public string GetIntegralString(IList<string> identity, string separator)
        {
            var result = string.Join(separator, GetIntegralStrings(identity));
            return string.IsNullOrEmpty(result) ? identity[0] : result;
        }

        /// <summary>
        /// Gets the formatted integral digit sequence.
        /// </summary>
        public string GetIntegralString(NumeralAlphabet alphabet, string separator = "")
        {
            if (separator == null) throw new ArgumentNullException(nameof(separator));
            var result = string.Join(separator, GetIntegralStrings(alphabet));
            return string.IsNullOrEmpty(result) ? alphabet[0] : result;
        }

        /// The `Numeral` class represents a numeral in a specific numeral system.
        /// It provides immutable integral and fractional views and conversions to different types.
        /// @constructor Numeral
        /// @param numericSystem - The numeral system that the numeral belongs to.
        /// @param integral - The list of indices representing the integral part of the numeral.
        /// @param fractional - The list of indices representing the fractional part of the numeral. (Optional)
        /// @param positive - Whether the numeral is positive or negative. (Default: true)
        /// /
        public Numeral()
        {
            Base = Numeral.System.OfBase(10);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        /// The `Numeral` class represents a numeral in a specific numeral system.
        /// @namespace NumeralSystems.Net
        /// @see NumeralSystem
        /// /
        public Numeral(NumeralSystem numericSystem)
        {
            Base = numericSystem ?? throw new ArgumentNullException(nameof(numericSystem));
        }

        /// <summary>
        /// Represents a numerical value in a specific numeral system.
        /// </summary>
        public Numeral(
            NumeralSystem numericSystem,
            List<int> integral,
            List<int>? fractional = null,
            bool positive = true)
            : this(numericSystem, integral, fractional, positive, null)
        {
        }

        /// <summary>
        /// Creates an immutable numeral projection while optionally preserving a
        /// separate exact rational value, such as for a repeating expansion.
        /// </summary>
        public static Numeral FromRepresentation(
            NumeralSystem numeralSystem,
            IEnumerable<int> integral,
            IEnumerable<int>? fractional,
            bool positive,
            RationalValue? exactValue = null)
        {
            if (integral is null) throw new ArgumentNullException(nameof(integral));
            return new Numeral(
                numeralSystem,
                integral.ToList(),
                fractional?.ToList(),
                positive,
                exactValue);
        }

        private Numeral(
            NumeralSystem numericSystem,
            List<int> integral,
            List<int>? fractional,
            bool positive,
            RationalValue? exactValue)
        {
            Base = numericSystem ?? throw new ArgumentNullException(nameof(numericSystem));
            if (integral is null) throw new ArgumentNullException(nameof(integral));
            if (!Base.Contains(integral))
                throw new ArgumentOutOfRangeException(nameof(integral), "The integral digits are invalid for the numeral base.");
            if (fractional is not null && !Base.Contains(fractional))
                throw new ArgumentOutOfRangeException(nameof(fractional), "The fractional digits are invalid for the numeral base.");
            if (exactValue is not null && !exactValue.IsZero && (exactValue.Sign > 0) != positive)
                throw new ArgumentException("The exact value sign must agree with the representation sign.", nameof(exactValue));

            _integralIndices.AddRange(integral);
            if (fractional is not null) _fractionalIndices.AddRange(fractional);
            _positive = positive;
            _exactValueOverride = exactValue;

            if (!Base.AdjustToFitIntegralLength) return;
            var difference = Base.Length - _integralIndices.Count;
            if (difference > 0)
                _integralIndices.InsertRange(0, Enumerable.Repeat(0, difference));
        }

        /// <summary>
        /// Represents a numeral value in a specific numeral system.
        /// </summary>
        public int Integer
            => checked((int)ExactValue.Truncate());

        /// <summary>
        /// Gets the arbitrary-precision integral value of this numeral.
        /// Fractional digits are truncated when reading the property.
        /// </summary>
        public BigInt BigInteger
            => ExactValue.Truncate();

        /// <summary>
        /// Represents a numeral in a specific numeral system.
        /// </summary>
        public char Char
            => checked((char)(ushort)ExactValue.Truncate());

        /// <summary>
        /// Represents a double-precision floating-point number.
        /// </summary>
        /// <remarks>
        /// Converts the exact numeral value to a double-precision floating-point number.
        /// </remarks>
        public double Double
            => (double)ExactValue.Numerator / (double)ExactValue.Denominator;

        /// <summary>
        /// Represents a numeral in a number system.
        /// </summary>
        public decimal Decimal
            => ExactValue.ToDecimal();

        /// <summary>
        /// Represents a numeral object that can store and manipulate numbers in different numeral systems.
        /// </summary>
        public float Float
            => decimal.ToSingle(Decimal);

        /// <summary>
        /// Gets the decimal byte-array representation of the Numeral value.
        /// </summary>
        public byte[] Bytes
            => decimal.GetBits(Decimal).SelectMany(BitConverter.GetBytes).ToArray();

        /// <summary>
        /// Creates a numeral as an immutable projection of an exact rational value.
        /// Repeating expansions keep the original rational value internally.
        /// </summary>
        public static Numeral FromRational(
            NumeralSystem numeralSystem,
            RationalValue value,
            NumeralConversionOptions? options = null)
        {
            if (numeralSystem is null) throw new ArgumentNullException(nameof(numeralSystem));
            if (value is null) throw new ArgumentNullException(nameof(value));
            var converted = NumeralValue.FromRational(
                value,
                numeralSystem.Size,
                options ?? NumeralConversionOptions.Default);
            return new Numeral(
                numeralSystem,
                converted.Integral.ToList(),
                converted.Decimals.ToList(),
                !converted.Negative,
                value);
        }

        /// <summary>Returns a new numeral in the same system with a different exact value.</summary>
        public Numeral WithExactValue(
            RationalValue value,
            NumeralConversionOptions? options = null) =>
            FromRational(Base, value, options);

        /// <summary>
        /// Converts the exact rational value to another numeral system without a
        /// floating-point or decimal intermediary.
        /// </summary>
        public Numeral To(NumeralSystem baseSystem, NumeralConversionOptions options)
        {
            if (baseSystem is null) throw new ArgumentNullException(nameof(baseSystem));
            if (options is null) throw new ArgumentNullException(nameof(options));

            return FromRational(baseSystem, ExactValue, options);
        }

        /// <summary>
        /// Represents a numeral in a specific numeral system.
        /// </summary>
        public string ToString(IList<string> identity, string separator, string negativeSign,
            string numberDecimalSeparator)
        {
            Base.TryFromIndices(IntegralIndices, FractionalIndices, identity, separator, negativeSign,
                numberDecimalSeparator, out var result, Positive);
            return result;
        }

        /// <summary>
        /// Formats this numeral with an ordered immutable alphabet.
        /// </summary>
        public string ToString(
            NumeralAlphabet alphabet,
            string separator = "",
            string negativeSign = "-",
            string numberDecimalSeparator = ".")
        {
            if (alphabet == null) throw new ArgumentNullException(nameof(alphabet));
            if (!Base.TryFromIndices(
                    IntegralIndices,
                    FractionalIndices,
                    alphabet,
                    separator,
                    negativeSign,
                    numberDecimalSeparator,
                    out var result,
                    Positive))
                throw new InvalidOperationException("The numeral contains a digit outside the alphabet.");
            return result;
        }

        /// <summary>
        /// Formats this numeral with serialization settings.
        /// </summary>
        public string ToString(NumeralSystem.SerializationInfo serializationInfo)
        {
            if (serializationInfo == null) throw new ArgumentNullException(nameof(serializationInfo));
            var useAlphabet =
                serializationInfo.Alphabet != null &&
                (serializationInfo.Identity == null ||
                 serializationInfo.Identity.Count == 0 ||
                 serializationInfo.Identity.SequenceEqual(
                     serializationInfo.Alphabet.Symbols,
                     StringComparer.Ordinal));
            return useAlphabet
                ? ToString(
                    serializationInfo.Alphabet,
                    serializationInfo.Separator,
                    serializationInfo.NegativeSign,
                    serializationInfo.NumberDecimalSeparator)
                : ToString(
                    serializationInfo.Identity,
                    serializationInfo.Separator,
                    serializationInfo.NegativeSign,
                    serializationInfo.NumberDecimalSeparator);
        }

        /// <summary>
        /// Returns a string that represents the current object in a specific format using the default identity, separator, negative sign, and number decimal separator.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            var serializationInfo = NumeralSystem.SerializationInfo.OfBase(Base.Size);
            return ToString(serializationInfo);
        }

        /// <summary>
        /// Formats using a standard numeral format. <c>G</c> uses the supplied
        /// provider; <c>R</c> uses the invariant default alphabet and tokens.
        /// </summary>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            var normalized = string.IsNullOrEmpty(format)
                ? "G"
                : format.ToUpperInvariant();
            switch (normalized)
            {
                case "G":
                {
                    var information = NumeralFormatInfo.Resolve(Base.Size, formatProvider);
                    return ToString(
                        information.Alphabet,
                        information.DigitSeparator,
                        information.NegativeSign,
                        information.DecimalSeparator);
                }
                case "R":
                    return ToString(
                        NumeralAlphabet.CreateDefault(Base.Size),
                        string.Empty,
                        CultureInfo.InvariantCulture.NumberFormat.NegativeSign,
                        CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
                default:
                    throw new FormatException(
                        $"Format '{format}' is not supported. Use G or R.");
            }
        }

        /// <summary>Formats with a provider using the general (<c>G</c>) format.</summary>
        public string ToString(IFormatProvider formatProvider) => ToString("G", formatProvider);

#if NET8_0_OR_GREATER
        /// <summary>
        /// Attempts to format into a caller-provided span on modern .NET targets.
        /// </summary>
        public bool TryFormat(
            Span<char> destination,
            out int charsWritten,
            ReadOnlySpan<char> format,
            IFormatProvider provider)
        {
            var formatted = ToString(format.Length == 0 ? null : format.ToString(), provider);
            charsWritten = 0;
            if (formatted.Length > destination.Length) return false;
            formatted.AsSpan().CopyTo(destination);
            charsWritten = formatted.Length;
            return true;
        }
#endif

        private void EnsureAlphabet(NumeralAlphabet alphabet)
        {
            if (alphabet == null) throw new ArgumentNullException(nameof(alphabet));
            if (alphabet.Count != Base.Size)
                throw new ArgumentOutOfRangeException(
                    nameof(alphabet),
                    "Alphabet size must equal the numeral-system base.");
        }

        /// The `System` class is a collection of static nested classes that provides various properties related to numeral systems.
        /// The `System` class is part of the `Numeral` namespace.
        /// This class contains the following nested classes:
        /// - `Characters`: Provides collections of characters representing different types of characters used in numeral systems, such as numbers, letters, symbols, etc.
        /// - `OfBase`: Provides a method to create a new instance of the `NumeralSystem` class with a specified base.
        /// Example usage:
        /// ```csharp
        /// var base10 = Numeral.System.OfBase(10);
        /// ```
        /// /
        public static class System
        {
            /// <summary>
            /// The Characters class provides static properties that define various sets of characters.
            /// These sets include numbers, upper and lower case letters, symbols, and other printable and non-printable characters.
            /// The class is part of the NumeralSystems.Net namespace.
            /// </summary>
            /// <seealso cref="NumeralSystems.Net.Numeral.System"/>
            public static class Characters
            {
                /// <summary>
                /// Collection of numeric characters.
                /// </summary>
                public static readonly IEnumerable<char> Numbers = Enumerable.Range(char.MinValue, char.MaxValue + 1)
                    .Skip(48)
                    .Select(i => (char)i)
                    .Where(c => !char.IsControl(c)).Take(10);

                /// <summary>
                /// A collection of upper case letters.
                /// </summary>
                public static readonly IEnumerable<char> UpperLetters = Enumerable
                    .Range(char.MinValue, char.MaxValue + 1).Skip(65)
                    .Select(i => (char)i)
                    .Where(c => !char.IsControl(c)).Take(26);

                /// <summary>
                /// Provides a collection of lower case letters in the Unicode character set.
                /// </summary>
                public static readonly IEnumerable<char> LowerLetters = Enumerable
                    .Range(char.MinValue, char.MaxValue + 1).Skip(97)
                    .Select(i => (char)i)
                    .Where(c => !char.IsControl(c)).Take(26);

                /// The collection of symbols used in the Numeral System.
                /// The collection includes alphanumeric symbols, as well as special characters.
                /// <remarks>
                /// The SymbolsA collection is designed to include a wide range of characters,
                /// such as letters, numbers, and other symbols, that can be used in various numeral systems.
                /// The collection is created by combining the Numbers, UpperLetters, LowerLetters,
                /// and other specified collections of characters.
                /// The resulting collection is then filtered to remove duplicate characters.
                /// </remarks>
                public static IEnumerable<char> Symbols
                {
                    get
                    {
                        var others = Alphanumeric.ToList();
                        return Enumerable.Range(0, char.MaxValue + 1)
                            .Select(i => (char)i)
                            .Where(c => !char.IsControl(c))
                            .Where(c => !others.Contains(c));
                    }
                }


                /// <summary>
                /// Represents a collection of alphanumeric characters, which includes numbers, uppercase letters, and lowercase letters.
                /// </summary>
                public static readonly IEnumerable<char> Alphanumeric =
                    Numbers.Concat(UpperLetters).Concat(LowerLetters);

                /// <summary>
                /// The collection of uppercase alphanumeric characters.
                /// </summary>
                /// <remarks>
                /// This collection includes uppercase letters (A-Z) and numbers (0-9).
                /// </remarks>
                public static IEnumerable<char> AlphanumericUpper = Numbers.Concat(UpperLetters);

                /// <summary>
                /// The collection of lowercase alphanumeric characters.
                /// </summary>
                public static IEnumerable<char> AlphanumericLower = Numbers.Concat(LowerLetters);

                /// <summary>
                /// Represents a collection of alphanumeric symbols.
                /// </summary>
                public static IEnumerable<char> AlphanumericSymbols =
                    Numbers.Concat(UpperLetters).Concat(LowerLetters)
                        .Concat(Symbols);
                /// <summary>
                /// The set of printable characters.
                /// </summary>
                public static readonly IEnumerable<char> Printable = Numbers.Concat(UpperLetters)
                    .Concat(LowerLetters)
                    .Concat(Symbols)
                    .Distinct();

                /// <summary>
                /// Characters that can't be printed
                /// </summary>
                public static readonly IEnumerable<char> NotPrintable = Enumerable
                    .Range(char.MinValue, char.MaxValue + 1)
                    .Select(i => (char)i)
                    .Where(char.IsControl);

                /// <summary>
                /// Contains all printable and non-printable characters in the Numeral Systems library.
                /// </summary>
                public static readonly IEnumerable<char> All = Printable.Concat(NotPrintable);

                /// <summary>
                /// Represents a collection of white space characters.
                /// </summary>
                public static IEnumerable<char> WhiteSpaces =
                    Printable.Where(ch => string.IsNullOrWhiteSpace(Convert.ToString(ch)));

                /// <summary>
                /// Represents a point character.
                /// </summary>
                public const char Point = '.';

                /// <summary>
                /// Represents the Comma character (,).
                /// </summary>
                public const char Comma = ',';

                /// <summary>
                /// The minus symbol character (-) used in numeral systems.
                /// </summary>
                public const char Minus = '-';

                /// <summary>
                /// Represents the semicolon character (;).
                /// </summary>
                /// <remarks>
                /// Used to separate items in numeral formats.
                /// </remarks>
                public const char Semicolon = ';';
            }

            /// <summary>
            /// Returns a range of values based on the given value and identity sequence.
            /// </summary>
            /// <param name="value">The value to determine the range.</param>
            /// <param name="identity">The sequence of identity values.</param>
            /// <returns>An enumerable range of values.</returns>
            private static IEnumerable<string> ValueRange(int value, IEnumerable<string> identity)
            {
                var enumerable = identity.ToList();
                return Sequence.IdentityEnumerableOfSize(enumerable, value).Select(x => string.Join(string.Empty, x));
            }

            /// <summary>
            /// Creates a NumeralSystem object of a specified base.
            /// </summary>
            /// <param name="value">The base value of the NumeralSystem.</param>
            /// <returns>A NumeralSystem object of the specified base.</returns>
            public static NumeralSystem OfBase(int value) => new(value);
        }
    }
}
