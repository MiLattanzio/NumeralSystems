using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NumeralSystems.Net.Encoding
{
    /// <summary>
    /// Provides the experimental character-radix transformation historically
    /// exposed by <c>Type.Base.String</c>. This is not Base16, Base32, or Base64
    /// binary encoding and it is not numeral formatting with an alphabet.
    /// </summary>
    public static class CharacterRadixTransform
    {
        private const int MaximumUtf16Base = char.MaxValue + 1;
        private const int MaximumRuneDigitBase = 0xD800;

        /// <summary>
        /// Encodes each UTF-16 code unit as a fixed-width sequence of radix
        /// digits stored directly as UTF-16 code units.
        /// </summary>
        public static string EncodeUtf16(
            string value,
            int destinationBase,
            out int digitsPerCodeUnit)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            ValidateUtf16Base(destinationBase, nameof(destinationBase));
            if (value.Length == 0)
            {
                digitsPerCodeUnit = 0;
                return string.Empty;
            }

            var maximum = 0;
            foreach (var codeUnit in value)
                maximum = Math.Max(maximum, codeUnit);
            digitsPerCodeUnit = DigitCount(maximum, destinationBase);

            var builder = new StringBuilder(checked(value.Length * digitsPerCodeUnit));
            foreach (var codeUnit in value)
                AppendDigits(builder, codeUnit, destinationBase, digitsPerCodeUnit);
            return builder.ToString();
        }

        /// <summary>
        /// Decodes a sequence produced by
        /// <see cref="EncodeUtf16(string, int, out int)"/>.
        /// </summary>
        public static string DecodeUtf16(
            string value,
            int sourceBase,
            int digitsPerCodeUnit)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            ValidateUtf16Base(sourceBase, nameof(sourceBase));
            ValidateWidth(value.Length, digitsPerCodeUnit, nameof(digitsPerCodeUnit));
            if (value.Length == 0) return string.Empty;

            var builder = new StringBuilder(value.Length / digitsPerCodeUnit);
            for (var offset = 0; offset < value.Length; offset += digitsPerCodeUnit)
            {
                var decoded = DecodeDigits(value, offset, digitsPerCodeUnit, sourceBase);
                if (decoded > char.MaxValue)
                    throw new FormatException("A decoded UTF-16 code unit exceeds U+FFFF.");
                builder.Append((char)decoded);
            }
            return builder.ToString();
        }

#if NET8_0_OR_GREATER
        /// <summary>
        /// Encodes Unicode scalar values rather than individual UTF-16 code
        /// units. Unpaired surrogate code units are rejected.
        /// </summary>
        public static string EncodeRunes(
            string value,
            int destinationBase,
            out int digitsPerRune)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            ValidateRuneDigitBase(destinationBase, nameof(destinationBase));
            if (value.Length == 0)
            {
                digitsPerRune = 0;
                return string.Empty;
            }

            var runes = ReadRunes(value);
            var maximum = 0;
            foreach (var rune in runes)
                maximum = Math.Max(maximum, rune.Value);
            digitsPerRune = DigitCount(maximum, destinationBase);

            var builder = new StringBuilder(checked(runes.Count * digitsPerRune));
            foreach (var rune in runes)
                AppendDigits(builder, rune.Value, destinationBase, digitsPerRune);
            return builder.ToString();
        }

        /// <summary>
        /// Decodes Unicode scalar values produced by
        /// <see cref="EncodeRunes(string, int, out int)"/>.
        /// </summary>
        public static string DecodeRunes(
            string value,
            int sourceBase,
            int digitsPerRune)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            ValidateRuneDigitBase(sourceBase, nameof(sourceBase));
            ValidateWidth(value.Length, digitsPerRune, nameof(digitsPerRune));
            if (value.Length == 0) return string.Empty;

            var builder = new StringBuilder();
            for (var offset = 0; offset < value.Length; offset += digitsPerRune)
            {
                var scalar = DecodeDigits(value, offset, digitsPerRune, sourceBase);
                if (!Rune.IsValid(scalar))
                    throw new FormatException($"The decoded value U+{scalar:X} is not a Unicode scalar value.");
                builder.Append(new Rune(scalar).ToString());
            }
            return builder.ToString();
        }
#endif

        /// <summary>
        /// Returns the smallest positional base that can contain every UTF-16
        /// code unit as a digit. The result is strictly greater than the maximum
        /// code-unit value and is 2 for an empty string.
        /// </summary>
        public static int GetSmallestBaseUtf16(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            var maximum = 0;
            foreach (var codeUnit in value)
                maximum = Math.Max(maximum, codeUnit);
            return Math.Max(2, maximum + 1);
        }

#if NET8_0_OR_GREATER
        /// <summary>
        /// Returns the smallest positional base that can contain every Unicode
        /// scalar as a digit. The result is strictly greater than the maximum
        /// scalar value and is 2 for an empty string.
        /// </summary>
        public static int GetSmallestBaseRunes(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            var maximum = 0;
            foreach (var rune in ReadRunes(value))
                maximum = Math.Max(maximum, rune.Value);
            return Math.Max(2, maximum + 1);
        }
#endif

        /// <summary>
        /// Streams UTF-16 code units using a caller-selected fixed width. The
        /// width must be large enough for every input code unit.
        /// </summary>
        public static void EncodeUtf16(
            TextReader input,
            TextWriter output,
            int destinationBase,
            int digitsPerCodeUnit)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (output == null) throw new ArgumentNullException(nameof(output));
            ValidateUtf16Base(destinationBase, nameof(destinationBase));
            ValidatePositiveWidth(digitsPerCodeUnit, nameof(digitsPerCodeUnit));

            int current;
            while ((current = input.Read()) >= 0)
                WriteDigits(output, current, destinationBase, digitsPerCodeUnit);
        }

        /// <summary>
        /// Streams and decodes fixed-width UTF-16 radix digits.
        /// </summary>
        public static void DecodeUtf16(
            TextReader input,
            TextWriter output,
            int sourceBase,
            int digitsPerCodeUnit)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (output == null) throw new ArgumentNullException(nameof(output));
            ValidateUtf16Base(sourceBase, nameof(sourceBase));
            ValidatePositiveWidth(digitsPerCodeUnit, nameof(digitsPerCodeUnit));

            while (TryReadDigits(input, sourceBase, digitsPerCodeUnit, out var decoded))
            {
                if (decoded > char.MaxValue)
                    throw new FormatException("A decoded UTF-16 code unit exceeds U+FFFF.");
                output.Write((char)decoded);
            }
        }

#if NET8_0_OR_GREATER
        /// <summary>
        /// Streams Unicode scalar values. Unpaired surrogate code units are
        /// rejected instead of being silently replaced.
        /// </summary>
        public static void EncodeRunes(
            TextReader input,
            TextWriter output,
            int destinationBase,
            int digitsPerRune)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (output == null) throw new ArgumentNullException(nameof(output));
            ValidateRuneDigitBase(destinationBase, nameof(destinationBase));
            ValidatePositiveWidth(digitsPerRune, nameof(digitsPerRune));

            while (TryReadRune(input, out var rune))
                WriteDigits(output, rune.Value, destinationBase, digitsPerRune);
        }

        /// <summary>
        /// Streams and decodes fixed-width Rune radix digits.
        /// </summary>
        public static void DecodeRunes(
            TextReader input,
            TextWriter output,
            int sourceBase,
            int digitsPerRune)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (output == null) throw new ArgumentNullException(nameof(output));
            ValidateRuneDigitBase(sourceBase, nameof(sourceBase));
            ValidatePositiveWidth(digitsPerRune, nameof(digitsPerRune));

            while (TryReadDigits(input, sourceBase, digitsPerRune, out var scalar))
            {
                if (!Rune.IsValid(scalar))
                    throw new FormatException($"The decoded value U+{scalar:X} is not a Unicode scalar value.");
                output.Write(new Rune(scalar).ToString());
            }
        }
#endif

#if NET8_0_OR_GREATER
        private static List<Rune> ReadRunes(string value)
        {
            var result = new List<Rune>();
            for (var index = 0; index < value.Length; index++)
            {
                var first = value[index];
                if (!char.IsSurrogate(first))
                {
                    result.Add(new Rune(first));
                    continue;
                }

                if (!char.IsHighSurrogate(first) || index + 1 >= value.Length ||
                    !Rune.TryCreate(first, value[index + 1], out var rune))
                    throw new ArgumentException(
                        $"The input contains an unpaired surrogate at UTF-16 position {index}.",
                        nameof(value));
                result.Add(rune);
                index++;
            }
            return result;
        }

        private static bool TryReadRune(TextReader input, out Rune rune)
        {
            var firstValue = input.Read();
            if (firstValue < 0)
            {
                rune = default;
                return false;
            }

            var first = (char)firstValue;
            if (!char.IsSurrogate(first))
            {
                rune = new Rune(first);
                return true;
            }
            if (!char.IsHighSurrogate(first))
                throw new FormatException("The input contains an unpaired low surrogate.");

            var secondValue = input.Read();
            if (secondValue < 0 || !Rune.TryCreate(first, (char)secondValue, out rune))
                throw new FormatException("The input contains an unpaired high surrogate.");
            return true;
        }
#endif

        private static void AppendDigits(
            StringBuilder builder,
            int value,
            int baseValue,
            int width)
        {
            var digits = ToDigits(value, baseValue, width);
            foreach (var digit in digits)
                builder.Append((char)digit);
        }

        private static void WriteDigits(
            TextWriter output,
            int value,
            int baseValue,
            int width)
        {
            foreach (var digit in ToDigits(value, baseValue, width))
                output.Write((char)digit);
        }

        private static int[] ToDigits(int value, int baseValue, int width)
        {
            var required = DigitCount(value, baseValue);
            if (required > width)
                throw new ArgumentOutOfRangeException(
                    nameof(width),
                    $"Width {width} cannot represent value {value} in base {baseValue}.");

            var result = new int[width];
            for (var position = width - 1; position >= 0; position--)
            {
                result[position] = value % baseValue;
                value /= baseValue;
            }
            return result;
        }

        private static int DecodeDigits(
            string value,
            int offset,
            int width,
            int baseValue)
        {
            var decoded = 0;
            for (var index = 0; index < width; index++)
            {
                var digit = value[offset + index];
                if (digit >= baseValue)
                    throw new FormatException(
                        $"Digit value {digit} at UTF-16 position {offset + index} is not valid in base {baseValue}.");
                decoded = checked(decoded * baseValue + digit);
            }
            return decoded;
        }

        private static bool TryReadDigits(
            TextReader input,
            int baseValue,
            int width,
            out int decoded)
        {
            decoded = 0;
            for (var index = 0; index < width; index++)
            {
                var digit = input.Read();
                if (digit < 0)
                {
                    if (index == 0) return false;
                    throw new FormatException("The final encoded unit is incomplete.");
                }
                if (digit >= baseValue)
                    throw new FormatException($"Digit value {digit} is not valid in base {baseValue}.");
                decoded = checked(decoded * baseValue + digit);
            }
            return true;
        }

        private static int DigitCount(int value, int baseValue)
        {
            var count = 1;
            while (value >= baseValue)
            {
                value /= baseValue;
                count++;
            }
            return count;
        }

        private static void ValidateWidth(int length, int width, string parameterName)
        {
            if (length == 0)
            {
                if (width < 0)
                    throw new ArgumentOutOfRangeException(parameterName, "Width cannot be negative.");
                return;
            }
            ValidatePositiveWidth(width, parameterName);
            if (length % width != 0)
                throw new FormatException("The encoded length is not divisible by the unit width.");
        }

        private static void ValidatePositiveWidth(int width, string parameterName)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException(parameterName, "Width must be greater than zero.");
        }

        private static void ValidateUtf16Base(int baseValue, string parameterName)
        {
            if (baseValue < 2 || baseValue > MaximumUtf16Base)
                throw new ArgumentOutOfRangeException(
                    parameterName,
                    $"Base must be between 2 and {MaximumUtf16Base}.");
        }

        private static void ValidateRuneDigitBase(int baseValue, string parameterName)
        {
            if (baseValue < 2 || baseValue > MaximumRuneDigitBase)
                throw new ArgumentOutOfRangeException(
                    parameterName,
                    $"Rune digit base must be between 2 and {MaximumRuneDigitBase}; " +
                    "this keeps every emitted digit a valid Unicode scalar.");
        }
    }
}
