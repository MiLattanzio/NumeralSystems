using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.Json;
using NumeralSystems.Net;
using NumeralSystems.Net.Encoding;
using NUnit.Framework;

namespace NumeralSystem.Net.NUnit
{
    [TestFixture]
    public class Encoding48Tests
    {
        [Test]
        public void EmptyInputsHaveDefinedResults()
        {
            var utf16 = CharacterRadixTransform.EncodeUtf16(string.Empty, 16, out var utf16Width);
            var runes = CharacterRadixTransform.EncodeRunes(string.Empty, 16, out var runeWidth);
            Assert.Multiple(() =>
            {
                Assert.That(utf16, Is.Empty);
                Assert.That(utf16Width, Is.Zero);
                Assert.That(CharacterRadixTransform.DecodeUtf16(string.Empty, 16, 0), Is.Empty);
                Assert.That(runes, Is.Empty);
                Assert.That(runeWidth, Is.Zero);
                Assert.That(CharacterRadixTransform.DecodeRunes(string.Empty, 16, 0), Is.Empty);
                Assert.That(StandardBaseCodec.EncodeBase16(Array.Empty<byte>()), Is.Empty);
                Assert.That(StandardBaseCodec.DecodeBase64(string.Empty), Is.Empty);
                Assert.That(Value.FromUtf16String(string.Empty, true).Indices, Is.Empty);
                Assert.That(Value.FromRunes(string.Empty, true).Indices, Is.Empty);
                Assert.That(Value.FromString(string.Empty, NumeralAlphabet.Base16).Indices, Is.Empty);
            });
        }

        [Test]
        public void SmallestBaseIsStrictlyGreaterThanMaximumDigit()
        {
            var utf16Maximum = Value.FromUtf16String("\uFFFF");
            var encodedMaximum = CharacterRadixTransform.EncodeUtf16(
                "\uFFFF",
                char.MaxValue + 1,
                out var maximumWidth);
            Assert.Multiple(() =>
            {
                Assert.That(CharacterRadixTransform.GetSmallestBaseUtf16(string.Empty), Is.EqualTo(2));
                Assert.That(CharacterRadixTransform.GetSmallestBaseUtf16("A"), Is.EqualTo('A' + 1));
                Assert.That(CharacterRadixTransform.GetSmallestBaseUtf16("\uFFFF"),
                    Is.EqualTo(char.MaxValue + 1));
                Assert.That(CharacterRadixTransform.GetSmallestBaseRunes("😀"),
                    Is.EqualTo(0x1F600 + 1));
                Assert.That(utf16Maximum.Base, Is.EqualTo(char.MaxValue + 1));
                Assert.That(utf16Maximum.ToUtf16String(), Is.EqualTo("\uFFFF"));
                Assert.That(maximumWidth, Is.EqualTo(1));
                Assert.That(CharacterRadixTransform.DecodeUtf16(
                    encodedMaximum,
                    char.MaxValue + 1,
                    maximumWidth), Is.EqualTo("\uFFFF"));
            });
        }

        [Test]
        public void RuneTransformTreatsSupplementaryCharactersAsOneScalar()
        {
            const string text = "A😀𝄞";
            var utf16Encoded = CharacterRadixTransform.EncodeUtf16(text, 256, out var utf16Width);
            var runeEncoded = CharacterRadixTransform.EncodeRunes(text, 256, out var runeWidth);

            Assert.Multiple(() =>
            {
                Assert.That(utf16Width, Is.EqualTo(2));
                Assert.That(utf16Encoded.Length, Is.EqualTo(text.Length * utf16Width));
                Assert.That(CharacterRadixTransform.DecodeUtf16(utf16Encoded, 256, utf16Width),
                    Is.EqualTo(text));
                Assert.That(runeWidth, Is.EqualTo(3));
                Assert.That(runeEncoded.Length, Is.EqualTo(3 * runeWidth));
                Assert.That(CharacterRadixTransform.DecodeRunes(runeEncoded, 256, runeWidth),
                    Is.EqualTo(text));
                Assert.That(Value.FromUtf16String("😀").Indices.Count, Is.EqualTo(2));
                Assert.That(Value.FromRunes("😀").Indices.Count, Is.EqualTo(1));
                Assert.That(Value.FromRunes(text).ToRuneString(), Is.EqualTo(text));
                Assert.That(CharacterIdentity.GetUtf16CodeUnits("😀😀").Count, Is.EqualTo(2));
                Assert.That(CharacterIdentity.GetRunes("😀😀").Count, Is.EqualTo(1));
            });
        }

        [Test]
        public void RuneTransformRejectsUnpairedSurrogates()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentException>(() =>
                    CharacterRadixTransform.EncodeRunes("\uD800", 16, out _));
                Assert.Throws<ArgumentException>(() => Value.FromRunes("\uDC00"));
            });
        }

        [TestCase("", "", "")]
        [TestCase("f", "66", "MY======")]
        [TestCase("fo", "666F", "MZXQ====")]
        [TestCase("foo", "666F6F", "MZXW6===")]
        [TestCase("foob", "666F6F62", "MZXW6YQ=")]
        [TestCase("fooba", "666F6F6261", "MZXW6YTB")]
        [TestCase("foobar", "666F6F626172", "MZXW6YTBOI======")]
        public void StandardBase16AndBase32MatchRfcVectors(
            string text,
            string base16,
            string base32)
        {
            var bytes = System.Text.Encoding.ASCII.GetBytes(text);
            Assert.Multiple(() =>
            {
                Assert.That(StandardBaseCodec.EncodeBase16(bytes), Is.EqualTo(base16));
                Assert.That(StandardBaseCodec.DecodeBase16(base16), Is.EqualTo(bytes));
                Assert.That(StandardBaseCodec.EncodeBase32(bytes), Is.EqualTo(base32));
                Assert.That(StandardBaseCodec.DecodeBase32(base32), Is.EqualTo(bytes));
                Assert.That(StandardBaseCodec.DecodeBase32(base32.TrimEnd('=')), Is.EqualTo(bytes));
            });
        }

        [TestCase("")]
        [TestCase("f")]
        [TestCase("fo")]
        [TestCase("foo")]
        [TestCase("foob")]
        [TestCase("fooba")]
        [TestCase("foobar")]
        public void StandardBase64MatchesFramework(string text)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(text);
            var expected = Convert.ToBase64String(bytes);
            Assert.Multiple(() =>
            {
                Assert.That(StandardBaseCodec.EncodeBase64(bytes), Is.EqualTo(expected));
                Assert.That(StandardBaseCodec.DecodeBase64(expected), Is.EqualTo(bytes));
                Assert.That(StandardBaseCodec.DecodeBase64(expected.TrimEnd('=')), Is.EqualTo(bytes));
            });
        }

        [Test]
        public void StandardCodecsRejectMalformedFinalBlocks()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<FormatException>(() => StandardBaseCodec.DecodeBase16("F"));
                Assert.Throws<FormatException>(() => StandardBaseCodec.DecodeBase32("A"));
                Assert.Throws<FormatException>(() => StandardBaseCodec.DecodeBase32("MZ======"));
                Assert.Throws<FormatException>(() => StandardBaseCodec.DecodeBase64("A"));
                Assert.Throws<FormatException>(() => StandardBaseCodec.DecodeBase64("Zh=="));
                Assert.Throws<FormatException>(() => StandardBaseCodec.DecodeBase64("Zg="));
            });
        }

        [TestCase(StandardBaseEncoding.Base16)]
        [TestCase(StandardBaseEncoding.Base32)]
        [TestCase(StandardBaseEncoding.Base64)]
        public void StandardCodecStreamingRoundTripsLargeInputs(StandardBaseEncoding encoding)
        {
            var data = new byte[256 * 1024 + 13];
            new Random(0x480).NextBytes(data);

            using var source = new MemoryStream(data, false);
            using var encodedWriter = new StringWriter(CultureInfo.InvariantCulture);
            StandardBaseCodec.Encode(source, encodedWriter, encoding, true, 997);
            var encoded = encodedWriter.ToString();

            using var encodedReader = new StringReader(encoded);
            using var decoded = new MemoryStream();
            StandardBaseCodec.Decode(encodedReader, decoded, encoding, 1013);

            Assert.That(decoded.ToArray(), Is.EqualTo(data));
        }

        [Test]
        public void CharacterTransformStreamingRoundTripsLargeRuneInput()
        {
            var text = string.Concat(Enumerable.Repeat("A😀𝄞", 10000));
            using var input = new StringReader(text);
            using var encodedWriter = new StringWriter(CultureInfo.InvariantCulture);
            CharacterRadixTransform.EncodeRunes(input, encodedWriter, 256, 3);

            using var encodedReader = new StringReader(encodedWriter.ToString());
            using var decodedWriter = new StringWriter(CultureInfo.InvariantCulture);
            CharacterRadixTransform.DecodeRunes(encodedReader, decodedWriter, 256, 3);

            Assert.That(decodedWriter.ToString(), Is.EqualTo(text));
        }

        [Test]
        public void NumeralSupportsFormatProvidersAndStandardFormats()
        {
            var system = Numeral.System.OfBase(16);
            system.AdjustToFitIntegralLength = false;
            var provider = new NumeralFormatInfo(
                NumeralAlphabet.Base16,
                "|",
                "~",
                ",");
            var parsed = system.Parse("~A|F,B", provider);

            Assert.Multiple(() =>
            {
                Assert.That(parsed.ToString("G", provider), Is.EqualTo("~A|F,B"));
                Assert.That(parsed.ToString("R", provider), Is.EqualTo("-AF.B"));
                Assert.That(
                    system.Parse(parsed.ToString("G", provider), provider).IntegralIndices,
                    Is.EqualTo(parsed.IntegralIndices));
                Assert.That(
                    system.Parse(parsed.ToString("G", provider), provider).FractionalIndices,
                    Is.EqualTo(parsed.FractionalIndices));
                Assert.That(NumberFormatInfo.GetInstance(provider).NumberDecimalSeparator,
                    Is.EqualTo(","));
            });
        }

        [Test]
        public void ModernSpanOverloadsRoundTrip()
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes("span 😀");
            Span<char> encoded = stackalloc char[64];
            Assert.That(StandardBaseCodec.TryEncode(
                bytes,
                encoded,
                out var charactersWritten,
                StandardBaseEncoding.Base64), Is.True);

            Span<byte> decoded = stackalloc byte[bytes.Length];
            Assert.That(StandardBaseCodec.TryDecode(
                encoded[..charactersWritten],
                decoded,
                out var bytesWritten,
                StandardBaseEncoding.Base64), Is.True);

            Span<char> numeralBuffer = stackalloc char[64];
            var number = Numeral.System.OfBase(16)[new BigInteger(-65535)];
            Assert.That(NumeralAlphabet.Base16.TryEncode(
                number.BigInteger,
                numeralBuffer,
                out var numeralLength), Is.True);
            Assert.That(NumeralAlphabet.Base16.Decode(numeralBuffer[..numeralLength]),
                Is.EqualTo(number.BigInteger));
            Assert.That(Numeral.System.OfBase(16).TryParse(
                numeralBuffer[..numeralLength],
                NumeralAlphabet.Base16).Success, Is.True);

            Span<char> formatted = stackalloc char[64];
            Assert.That(number.TryFormat(formatted, out var formattedLength, "R", null), Is.True);
            var decodedBytes = decoded[..bytesWritten].ToArray();
            var formattedText = formatted[..formattedLength].ToString();

            Assert.Multiple(() =>
            {
                Assert.That(decodedBytes, Is.EqualTo(bytes));
                Assert.That(formattedText, Is.EqualTo("-FFFF"));
            });
        }

        [Test]
        public void JsonSerializationPreservesBaseSignAndExactDigits()
        {
            var system = Numeral.System.OfBase(16);
            system.AdjustToFitIntegralLength = false;
            var value = new Numeral(system, new[] { 0, 15 }.ToList(), new[] { 0, 1 }.ToList(), false);

            var json = JsonSerializer.Serialize(value);
            var roundTrip = JsonSerializer.Deserialize<Numeral>(json);

            Assert.That(roundTrip, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(roundTrip.Base.Size, Is.EqualTo(16));
                Assert.That(roundTrip.Positive, Is.False);
                Assert.That(roundTrip.IntegralIndices, Is.EqualTo(new[] { 0, 15 }));
                Assert.That(roundTrip.FractionalIndices, Is.EqualTo(new[] { 0, 1 }));
            });
        }

        [Test]
        public void JsonSerializationRejectsDigitsOutsideTheBase()
        {
            const string json =
                "{\"base\":2,\"positive\":true,\"integral\":[2],\"fractional\":[]}";
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Numeral>(json));
        }

        [Test]
        public void JsonSerializationPreservesEmptyDigitArrays()
        {
            var value = new Numeral();
            var roundTrip = JsonSerializer.Deserialize<Numeral>(JsonSerializer.Serialize(value));

            Assert.That(roundTrip, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(roundTrip.Base.Size, Is.EqualTo(10));
                Assert.That(roundTrip.IntegralIndices, Is.Empty);
                Assert.That(roundTrip.FractionalIndices, Is.Empty);
            });
        }
    }
}
