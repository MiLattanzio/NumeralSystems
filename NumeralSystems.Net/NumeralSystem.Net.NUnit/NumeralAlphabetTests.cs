using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NumeralSystems.Net;
using NUnit.Framework;

namespace NumeralSystem.Net.NUnit
{
    [TestFixture]
    public class NumeralAlphabetTests
    {
        private static readonly NumeralAlphabet[] Predefined =
        {
            NumeralAlphabet.Base2,
            NumeralAlphabet.Base8,
            NumeralAlphabet.Base10,
            NumeralAlphabet.Base16,
            NumeralAlphabet.Base32,
            NumeralAlphabet.Base36,
            NumeralAlphabet.Base58,
            NumeralAlphabet.Base62,
            NumeralAlphabet.Base64
        };

        [Test]
        public void PredefinedAlphabetsHaveStableOrderedSymbols()
        {
            Assert.Multiple(() =>
            {
                Assert.That(NumeralAlphabet.Base2.ToString(), Is.EqualTo("01"));
                Assert.That(NumeralAlphabet.Base8.ToString(), Is.EqualTo("01234567"));
                Assert.That(NumeralAlphabet.Base10.ToString(), Is.EqualTo("0123456789"));
                Assert.That(NumeralAlphabet.Base16.ToString(), Is.EqualTo("0123456789ABCDEF"));
                Assert.That(
                    NumeralAlphabet.Base32.ToString(),
                    Is.EqualTo("0123456789ABCDEFGHJKMNPQRSTVWXYZ"));
                Assert.That(
                    NumeralAlphabet.Base36.ToString(),
                    Is.EqualTo("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
                Assert.That(
                    NumeralAlphabet.Base58.ToString(),
                    Is.EqualTo("123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz"));
                Assert.That(
                    NumeralAlphabet.Base62.ToString(),
                    Is.EqualTo("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"));
                Assert.That(
                    NumeralAlphabet.Base64.ToString(),
                    Is.EqualTo("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/"));
                Assert.That(
                    Predefined.Select(alphabet => alphabet.Count),
                    Is.EqualTo(NumeralAlphabet.PredefinedBases));
            });
        }

        [Test]
        public void AlphabetCopiesInputAndUsesOrdinalLookup()
        {
            var symbols = new List<string> { "zero", "ONE" };
            var alphabet = new NumeralAlphabet(symbols);
            symbols[0] = "changed";

            Assert.Multiple(() =>
            {
                Assert.That(alphabet[0], Is.EqualTo("zero"));
                Assert.That(alphabet.IndexOf("ONE"), Is.EqualTo(1));
                Assert.That(alphabet.IndexOf("one"), Is.EqualTo(-1));
                Assert.That(alphabet.Contains("zero"), Is.True);
            });
        }

        [Test]
        public void InvalidAndAmbiguousSymbolsAreRejected()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentException>(() => new NumeralAlphabet(new[] { "0" }));
                Assert.Throws<ArgumentException>(() => new NumeralAlphabet(new[] { "0", "" }));
                Assert.Throws<ArgumentException>(() => new NumeralAlphabet(new[] { "0", "0" }));
                Assert.Throws<ArgumentException>(() => new NumeralAlphabet(new[] { "a", "ab" }));
                Assert.Throws<ArgumentException>(() => new NumeralAlphabet(new[] { "xyz", "x" }));
            });
        }

        [Test]
        public void SeparatorAndSignConflictsAreRejected()
        {
            var normal = NumeralAlphabet.Base10;
            var signDigit = new NumeralAlphabet(new[] { "0", "-" });
            var separatorDigit = new NumeralAlphabet(new[] { "0", "|" });

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() => normal.ValidateFormat("", "-", "."));
                Assert.Throws<ArgumentException>(() => signDigit.ValidateFormat("", "-", "."));
                Assert.Throws<ArgumentException>(() => separatorDigit.ValidateFormat("|", "-", "."));
                Assert.Throws<ArgumentException>(() => normal.ValidateFormat("::", ":", "."));
            });
        }

        [Test]
        public void ParseResultReportsReasonAndUtf16Position()
        {
            var hexadecimal = Numeral.System.OfBase(16);

            AssertFailure(hexadecimal.TryParse(null, NumeralAlphabet.Base16),
                ParseErrorReason.NullInput, 0);
            AssertFailure(hexadecimal.TryParse("", NumeralAlphabet.Base16),
                ParseErrorReason.EmptyInput, 0);
            AssertFailure(hexadecimal.TryParse("1G", NumeralAlphabet.Base16),
                ParseErrorReason.UnknownSymbol, 1);
            AssertFailure(hexadecimal.TryParse("-1-", NumeralAlphabet.Base16),
                ParseErrorReason.MisplacedNegativeSign, 2);
            AssertFailure(hexadecimal.TryParse("1.2.3", NumeralAlphabet.Base16),
                ParseErrorReason.RepeatedDecimalSeparator, 3);
            AssertFailure(hexadecimal.TryParse(".1", NumeralAlphabet.Base16),
                ParseErrorReason.MissingDigit, 0);
            AssertFailure(hexadecimal.TryParse("1.", NumeralAlphabet.Base16),
                ParseErrorReason.MissingDigit, 2);
            AssertFailure(hexadecimal.TryParse("1||2", NumeralAlphabet.Base16, "|"),
                ParseErrorReason.UnexpectedSeparator, 2);
            AssertFailure(hexadecimal.TryParse("|1", NumeralAlphabet.Base16, "|"),
                ParseErrorReason.UnexpectedSeparator, 0);
            AssertFailure(hexadecimal.TryParse("12", NumeralAlphabet.Base16, "|"),
                ParseErrorReason.MissingSeparator, 1);
            AssertFailure(
                hexadecimal.TryParse("10", NumeralAlphabet.Base10),
                ParseErrorReason.AlphabetSizeMismatch,
                0);
            AssertFailure(
                Numeral.System.OfBase(2).TryParse(
                    "0",
                    new NumeralAlphabet(new[] { "0", "-" })),
                ParseErrorReason.InvalidConfiguration,
                0);

            var invalidLegacySettings = new NumeralSystems.Net.NumeralSystem.SerializationInfo
            {
                Identity = new List<string> { "0", "0" },
                NegativeSign = "-",
                NumberDecimalSeparator = "."
            };
            AssertFailure(
                Numeral.System.OfBase(2).TryParse("0", invalidLegacySettings),
                ParseErrorReason.InvalidConfiguration,
                0);
        }

        [Test]
        public void ParseResultReturnsSignedFractionalNumeralOnSuccess()
        {
            var system = Numeral.System.OfBase(16);
            system.AdjustToFitIntegralLength = false;
            var parsed = system.TryParse("-A.F", NumeralAlphabet.Base16);

            Assert.Multiple(() =>
            {
                Assert.That(parsed.Success, Is.True);
                Assert.That(parsed.Reason, Is.EqualTo(ParseErrorReason.None));
                Assert.That(parsed.Position, Is.EqualTo(-1));
                Assert.That(parsed.Value.Positive, Is.False);
                Assert.That(parsed.Value.IntegralIndices, Is.EqualTo(new[] { 10 }));
                Assert.That(parsed.Value.FractionalIndices, Is.EqualTo(new[] { 15 }));
                Assert.That(parsed.Value.ToString(NumeralAlphabet.Base16), Is.EqualTo("-A.F"));
            });
        }

        [Test]
        public void FixedWidthGeneratedAlphabetRoundTripsWithoutASeparator()
        {
            var alphabet = NumeralAlphabet.CreateDefault(12);
            var value = BigInteger.Parse("123456789012345678901234567890");
            var encoded = alphabet.Encode(value);

            Assert.Multiple(() =>
            {
                Assert.That(alphabet[0], Is.EqualTo("00"));
                Assert.That(alphabet[11], Is.EqualTo("11"));
                Assert.That(alphabet.Decode(encoded), Is.EqualTo(value));
                Assert.That(Value.FromString(encoded, alphabet).ToBigInteger(), Is.EqualTo(value));
            });
        }

        [Test]
        public void PropertyEncodeDecodeRoundTripsForEveryBaseFromTwoThroughOneHundredTwentyEight()
        {
            var random = new Random(0xA1FA);
            for (var baseValue = 2; baseValue <= 128; baseValue++)
            {
                var alphabet = NumeralAlphabet.CreateDefault(baseValue);
                foreach (var value in GeneratedValues(random, 40))
                {
                    var encoded = alphabet.Encode(value);
                    Assert.That(
                        alphabet.Decode(encoded),
                        Is.EqualTo(value),
                        $"Base {baseValue}, text '{encoded}'");
                }
            }
        }

        [Test]
        public void PropertyNumeralConversionRoundTripsBetweenEveryPredefinedBasePair()
        {
            var random = new Random(0xB453);
            var values = GeneratedValues(random, 80).ToArray();

            foreach (var sourceAlphabet in Predefined)
            {
                var sourceSystem = Numeral.System.OfBase(sourceAlphabet.Count);
                foreach (var destinationAlphabet in Predefined)
                {
                    var destinationSystem = Numeral.System.OfBase(destinationAlphabet.Count);
                    foreach (var value in values)
                    {
                        var source = sourceSystem[value];
                        var sourceText = source.ToString(sourceAlphabet);
                        var sourceRoundTrip = sourceSystem.Parse(sourceText, sourceAlphabet);
                        var converted = sourceRoundTrip.To(destinationSystem, NumeralConversionOptions.Default);
                        var destinationText = converted.ToString(destinationAlphabet);
                        var destinationRoundTrip =
                            destinationSystem.Parse(destinationText, destinationAlphabet);

                        Assert.That(
                            destinationRoundTrip.BigInteger,
                            Is.EqualTo(value),
                            $"{sourceAlphabet.Count} -> {destinationAlphabet.Count}, '{destinationText}'");
                    }
                }
            }
        }

        [Test]
        public void ValuePreservesLeadingZeroSymbolsWithOrderedAlphabet()
        {
            var value = Value.FromString("0000FF", NumeralAlphabet.Base16);

            Assert.Multiple(() =>
            {
                Assert.That(value.ToString(NumeralAlphabet.Base16), Is.EqualTo("0000FF"));
                Assert.That(value.ToBigInteger(), Is.EqualTo(new BigInteger(255)));
            });
        }

        [Test]
        public void SerializationInfoUsesPredefinedAlphabetWhenAvailable()
        {
            var info = NumeralSystems.Net.NumeralSystem.SerializationInfo.OfBase(58);

            Assert.Multiple(() =>
            {
                Assert.That(info.Alphabet, Is.SameAs(NumeralAlphabet.Base58));
                Assert.That(info.Identity, Is.EqualTo(NumeralAlphabet.Base58.Symbols));
            });
        }

        [Test]
        public void ModifiedLegacyIdentityStillOverridesSerializationInfoAlphabet()
        {
            var system = Numeral.System.OfBase(2);
            system.AdjustToFitIntegralLength = false;
            var info = NumeralSystems.Net.NumeralSystem.SerializationInfo.OfBase(2);
            info.Identity = new List<string> { "x", "y" };

            var parsed = system.Parse("y", info);

            Assert.Multiple(() =>
            {
                Assert.That(parsed.BigInteger, Is.EqualTo(BigInteger.One));
                Assert.That(parsed.ToString(info), Is.EqualTo("y"));
            });
        }

        private static IEnumerable<BigInteger> GeneratedValues(Random random, int count)
        {
            yield return BigInteger.Zero;
            yield return BigInteger.One;
            yield return BigInteger.MinusOne;
            yield return BigInteger.Pow(2, 256) - 1;
            yield return -BigInteger.Pow(2, 256) + 1;

            var bytes = new byte[65];
            for (var index = 0; index < count; index++)
            {
                random.NextBytes(bytes);
                bytes[^1] = 0;
                var value = new BigInteger(bytes);
                yield return index % 2 == 0 ? value : BigInteger.Negate(value);
            }
        }

        private static void AssertFailure(
            ParseResult result,
            ParseErrorReason reason,
            int position)
        {
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Value, Is.Null);
                Assert.That(result.Reason, Is.EqualTo(reason));
                Assert.That(result.Position, Is.EqualTo(position));
                Assert.That(result.Message, Is.Not.Empty);
            });
        }
    }
}
