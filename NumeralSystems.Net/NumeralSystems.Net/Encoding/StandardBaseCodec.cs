using System;
using System.IO;

namespace NumeralSystems.Net.Encoding
{
    /// <summary>
    /// Identifies an RFC-compatible binary-to-text encoding. These encodings
    /// operate on bytes and are separate from <see cref="NumeralAlphabet"/>.
    /// </summary>
    public enum StandardBaseEncoding
    {
        /// <summary>RFC 4648 Base16 using uppercase hexadecimal characters.</summary>
        Base16,
        /// <summary>RFC 4648 Base32 using the A-Z, 2-7 alphabet.</summary>
        Base32,
        /// <summary>RFC 4648 Base64 using the standard + and / alphabet.</summary>
        Base64
    }

    /// <summary>
    /// Encodes and decodes byte sequences with standard Base16, Base32, and
    /// Base64 semantics, including streaming APIs for large inputs.
    /// </summary>
    public static class StandardBaseCodec
    {
        private const string Base16Alphabet = "0123456789ABCDEF";
        private const string Base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        private const string Base64Alphabet =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
        private const int DefaultBufferSize = 81920;

        /// <summary>Encodes bytes using the requested standard encoding.</summary>
        public static string Encode(
            byte[] data,
            StandardBaseEncoding encoding,
            bool includePadding = true)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            using var input = new MemoryStream(data, false);
            using var output = new StringWriter(System.Globalization.CultureInfo.InvariantCulture);
            Encode(input, output, encoding, includePadding);
            return output.ToString();
        }

        /// <summary>Decodes standard Base16, Base32, or Base64 text.</summary>
        public static byte[] Decode(string text, StandardBaseEncoding encoding)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            using var input = new StringReader(text);
            using var output = new MemoryStream();
            Decode(input, output, encoding);
            return output.ToArray();
        }

        /// <summary>Encodes bytes as RFC 4648 Base16.</summary>
        public static string EncodeBase16(byte[] data) =>
            Encode(data, StandardBaseEncoding.Base16, false);

        /// <summary>Decodes RFC 4648 Base16 text.</summary>
        public static byte[] DecodeBase16(string text) =>
            Decode(text, StandardBaseEncoding.Base16);

        /// <summary>Encodes bytes as padded RFC 4648 Base32.</summary>
        public static string EncodeBase32(byte[] data, bool includePadding = true) =>
            Encode(data, StandardBaseEncoding.Base32, includePadding);

        /// <summary>Decodes padded or unpadded RFC 4648 Base32 text.</summary>
        public static byte[] DecodeBase32(string text) =>
            Decode(text, StandardBaseEncoding.Base32);

        /// <summary>Encodes bytes as padded RFC 4648 Base64.</summary>
        public static string EncodeBase64(byte[] data, bool includePadding = true) =>
            Encode(data, StandardBaseEncoding.Base64, includePadding);

        /// <summary>Decodes padded or unpadded RFC 4648 Base64 text.</summary>
        public static byte[] DecodeBase64(string text) =>
            Decode(text, StandardBaseEncoding.Base64);

        /// <summary>
        /// Streams bytes to a text writer without buffering the complete input
        /// or output in memory. Streams and writers remain open.
        /// </summary>
        public static void Encode(
            Stream input,
            TextWriter output,
            StandardBaseEncoding encoding,
            bool includePadding = true,
            int bufferSize = DefaultBufferSize)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (output == null) throw new ArgumentNullException(nameof(output));
            if (!input.CanRead) throw new ArgumentException("The input stream must be readable.", nameof(input));
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize), "Buffer size must be positive.");

            var specification = GetSpecification(encoding);
            var inputBuffer = new byte[bufferSize];
            var outputBuffer = new char[4096];
            var outputLength = 0;
            ulong bitBuffer = 0;
            var bitCount = 0;
            var characterCount = 0;

            void FlushOutput()
            {
                if (outputLength == 0) return;
                output.Write(outputBuffer, 0, outputLength);
                outputLength = 0;
            }

            void WriteCharacter(char character)
            {
                if (outputLength == outputBuffer.Length) FlushOutput();
                outputBuffer[outputLength++] = character;
                characterCount++;
            }

            int read;
            while ((read = input.Read(inputBuffer, 0, inputBuffer.Length)) > 0)
            {
                for (var index = 0; index < read; index++)
                {
                    bitBuffer = (bitBuffer << 8) | inputBuffer[index];
                    bitCount += 8;
                    while (bitCount >= specification.BitsPerCharacter)
                    {
                        var shift = bitCount - specification.BitsPerCharacter;
                        var alphabetIndex = (int)((bitBuffer >> shift) & specification.Mask);
                        WriteCharacter(specification.Alphabet[alphabetIndex]);
                        bitCount -= specification.BitsPerCharacter;
                        bitBuffer = KeepLowBits(bitBuffer, bitCount);
                    }
                }
            }

            if (bitCount > 0)
            {
                var alphabetIndex = (int)((bitBuffer <<
                    (specification.BitsPerCharacter - bitCount)) & specification.Mask);
                WriteCharacter(specification.Alphabet[alphabetIndex]);
            }

            if (includePadding && specification.BlockCharacters > 1)
                while (characterCount % specification.BlockCharacters != 0)
                    WriteCharacter('=');
            FlushOutput();
        }

        /// <summary>
        /// Streams encoded text to a byte stream. ASCII whitespace is ignored;
        /// malformed symbols, padding, or non-zero unused bits are rejected.
        /// Streams and readers remain open.
        /// </summary>
        public static void Decode(
            TextReader input,
            Stream output,
            StandardBaseEncoding encoding,
            int bufferSize = DefaultBufferSize)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (output == null) throw new ArgumentNullException(nameof(output));
            if (!output.CanWrite) throw new ArgumentException("The output stream must be writable.", nameof(output));
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize), "Buffer size must be positive.");

            var specification = GetSpecification(encoding);
            var inputBuffer = new char[Math.Max(128, Math.Min(bufferSize, 81920))];
            var outputBuffer = new byte[Math.Max(128, Math.Min(bufferSize, 81920))];
            var outputLength = 0;
            ulong bitBuffer = 0;
            var bitCount = 0;
            var symbolCount = 0;
            var paddingCount = 0;
            var sawPadding = false;

            void FlushOutput()
            {
                if (outputLength == 0) return;
                output.Write(outputBuffer, 0, outputLength);
                outputLength = 0;
            }

            void WriteByte(byte value)
            {
                if (outputLength == outputBuffer.Length) FlushOutput();
                outputBuffer[outputLength++] = value;
            }

            int read;
            while ((read = input.Read(inputBuffer, 0, inputBuffer.Length)) > 0)
            {
                for (var position = 0; position < read; position++)
                {
                    var character = inputBuffer[position];
                    if (char.IsWhiteSpace(character)) continue;
                    if (character == '=')
                    {
                        if (specification.BlockCharacters == 1)
                            throw new FormatException("Base16 does not use padding.");
                        sawPadding = true;
                        paddingCount++;
                        continue;
                    }
                    if (sawPadding)
                        throw new FormatException("A non-padding symbol appears after padding.");

                    var value = DecodeCharacter(character, encoding);
                    if (value < 0)
                        throw new FormatException($"Character '{character}' is not valid {encoding} text.");

                    symbolCount++;
                    bitBuffer = (bitBuffer << specification.BitsPerCharacter) | (uint)value;
                    bitCount += specification.BitsPerCharacter;
                    while (bitCount >= 8)
                    {
                        var shift = bitCount - 8;
                        WriteByte((byte)(bitBuffer >> shift));
                        bitCount -= 8;
                        bitBuffer = KeepLowBits(bitBuffer, bitCount);
                    }
                }
            }

            ValidateFinalBlock(
                encoding,
                specification,
                symbolCount,
                paddingCount,
                bitBuffer,
                bitCount);
            FlushOutput();
        }

#if NET8_0_OR_GREATER
        /// <summary>Encodes a byte span on modern .NET targets.</summary>
        public static string Encode(
            ReadOnlySpan<byte> data,
            StandardBaseEncoding encoding,
            bool includePadding = true)
        {
            var length = GetEncodedLength(data.Length, encoding, includePadding);
            return string.Create(
                length,
                (Data: data.ToArray(), Encoding: encoding, Padding: includePadding),
                static (destination, state) =>
                {
                    if (!TryEncode(state.Data, destination, out var written, state.Encoding, state.Padding) ||
                        written != destination.Length)
                        throw new InvalidOperationException("The encoded-length calculation is inconsistent.");
                });
        }

        /// <summary>Attempts to encode bytes into a caller-provided character span.</summary>
        public static bool TryEncode(
            ReadOnlySpan<byte> data,
            Span<char> destination,
            out int charactersWritten,
            StandardBaseEncoding encoding,
            bool includePadding = true)
        {
            var required = GetEncodedLength(data.Length, encoding, includePadding);
            charactersWritten = 0;
            if (destination.Length < required) return false;

            var specification = GetSpecification(encoding);
            ulong bitBuffer = 0;
            var bitCount = 0;
            foreach (var item in data)
            {
                bitBuffer = (bitBuffer << 8) | item;
                bitCount += 8;
                while (bitCount >= specification.BitsPerCharacter)
                {
                    var shift = bitCount - specification.BitsPerCharacter;
                    destination[charactersWritten++] =
                        specification.Alphabet[(int)((bitBuffer >> shift) & specification.Mask)];
                    bitCount -= specification.BitsPerCharacter;
                    bitBuffer = KeepLowBits(bitBuffer, bitCount);
                }
            }
            if (bitCount > 0)
                destination[charactersWritten++] = specification.Alphabet[
                    (int)((bitBuffer << (specification.BitsPerCharacter - bitCount)) & specification.Mask)];
            if (includePadding && specification.BlockCharacters > 1)
                while (charactersWritten % specification.BlockCharacters != 0)
                    destination[charactersWritten++] = '=';
            return true;
        }

        /// <summary>Decodes a character span on modern .NET targets.</summary>
        public static byte[] Decode(ReadOnlySpan<char> text, StandardBaseEncoding encoding) =>
            Decode(text.ToString(), encoding);

        /// <summary>Attempts to decode into a caller-provided byte span.</summary>
        public static bool TryDecode(
            ReadOnlySpan<char> text,
            Span<byte> destination,
            out int bytesWritten,
            StandardBaseEncoding encoding)
        {
            try
            {
                var decoded = Decode(text.ToString(), encoding);
                bytesWritten = 0;
                if (decoded.Length > destination.Length) return false;
                decoded.CopyTo(destination);
                bytesWritten = decoded.Length;
                return true;
            }
            catch (FormatException)
            {
                bytesWritten = 0;
                return false;
            }
        }
#endif

        private static int GetEncodedLength(
            int byteCount,
            StandardBaseEncoding encoding,
            bool includePadding)
        {
            var specification = GetSpecification(encoding);
            var characters = byteCount == 0
                ? 0
                : checked((byteCount * 8 + specification.BitsPerCharacter - 1) /
                          specification.BitsPerCharacter);
            if (!includePadding || specification.BlockCharacters == 1 || characters == 0)
                return characters;
            return checked(
                ((characters + specification.BlockCharacters - 1) /
                 specification.BlockCharacters) * specification.BlockCharacters);
        }

        private static int DecodeCharacter(char character, StandardBaseEncoding encoding)
        {
            if (encoding == StandardBaseEncoding.Base16)
            {
                if (character >= '0' && character <= '9') return character - '0';
                if (character >= 'A' && character <= 'F') return character - 'A' + 10;
                if (character >= 'a' && character <= 'f') return character - 'a' + 10;
                return -1;
            }
            if (encoding == StandardBaseEncoding.Base32)
            {
                if (character >= 'A' && character <= 'Z') return character - 'A';
                if (character >= 'a' && character <= 'z') return character - 'a';
                if (character >= '2' && character <= '7') return character - '2' + 26;
                return -1;
            }
            if (character >= 'A' && character <= 'Z') return character - 'A';
            if (character >= 'a' && character <= 'z') return character - 'a' + 26;
            if (character >= '0' && character <= '9') return character - '0' + 52;
            return character switch
            {
                '+' => 62,
                '/' => 63,
                _ => -1
            };
        }

        private static void ValidateFinalBlock(
            StandardBaseEncoding encoding,
            CodecSpecification specification,
            int symbolCount,
            int paddingCount,
            ulong bitBuffer,
            int bitCount)
        {
            var remainder = symbolCount % specification.BlockCharacters;
            var expectedPadding = 0;
            var validRemainder = true;
            switch (encoding)
            {
                case StandardBaseEncoding.Base16:
                    validRemainder = remainder == 0;
                    break;
                case StandardBaseEncoding.Base32:
                    expectedPadding = remainder switch
                    {
                        0 => 0,
                        2 => 6,
                        4 => 4,
                        5 => 3,
                        7 => 1,
                        _ => -1
                    };
                    validRemainder = expectedPadding >= 0;
                    break;
                case StandardBaseEncoding.Base64:
                    expectedPadding = remainder switch
                    {
                        0 => 0,
                        2 => 2,
                        3 => 1,
                        _ => -1
                    };
                    validRemainder = expectedPadding >= 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(encoding));
            }

            if (!validRemainder)
                throw new FormatException($"The final {encoding} block has an invalid length.");
            if (paddingCount > 0 && paddingCount != expectedPadding)
                throw new FormatException($"The {encoding} padding length is invalid.");
            if (bitCount > 0 && bitBuffer != 0)
                throw new FormatException($"The unused bits in the final {encoding} symbol must be zero.");
        }

        private static ulong KeepLowBits(ulong value, int bitCount) =>
            bitCount == 0 ? 0 : value & ((1UL << bitCount) - 1);

        private static CodecSpecification GetSpecification(StandardBaseEncoding encoding) =>
            encoding switch
            {
                StandardBaseEncoding.Base16 => new CodecSpecification(Base16Alphabet, 4, 2),
                StandardBaseEncoding.Base32 => new CodecSpecification(Base32Alphabet, 5, 8),
                StandardBaseEncoding.Base64 => new CodecSpecification(Base64Alphabet, 6, 4),
                _ => throw new ArgumentOutOfRangeException(nameof(encoding))
            };

        private readonly struct CodecSpecification
        {
            internal CodecSpecification(string alphabet, int bitsPerCharacter, int blockCharacters)
            {
                Alphabet = alphabet;
                BitsPerCharacter = bitsPerCharacter;
                BlockCharacters = blockCharacters;
                Mask = (1UL << bitsPerCharacter) - 1;
            }

            internal string Alphabet { get; }
            internal int BitsPerCharacter { get; }
            internal int BlockCharacters { get; }
            internal ulong Mask { get; }
        }
    }
}
