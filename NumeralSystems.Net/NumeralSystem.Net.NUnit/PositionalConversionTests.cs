using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NumeralSystems.Net;
using NUnit.Framework;
using BaseBigInteger = NumeralSystems.Net.Type.Base.BigInteger;
using BaseDecimal = NumeralSystems.Net.Type.Base.Decimal;

namespace NumeralSystem.Net.NUnit
{
    [TestFixture]
    public class PositionalConversionTests
    {
        [Test]
        public void FractionalDigitsUseTheirDeclaredBase()
        {
            var oneHalf = NumeralValue.FromDigits(new[] { 0 }, new[] { 1 }, false, 2);

            Assert.That(oneHalf.ToDecimal(), Is.EqualTo(0.5m));
            Assert.That(oneHalf.ToDouble(), Is.EqualTo(0.5d));

            var numeral = new Numeral(
                Numeral.System.OfBase(2),
                new List<int> { 0 },
                new List<int> { 1 });
            Assert.That(numeral.Decimal, Is.EqualTo(0.5m));
            Assert.That(numeral.Double, Is.EqualTo(0.5d));
        }

        [Test]
        public void TerminatingFractionConvertsExactly()
        {
            var source = NumeralValue.FromDecimal(10.625m);

            var options = new NumeralConversionOptions(
                16,
                NumeralRoundingMode.ToZero,
                true,
                InfiniteExpansionBehavior.Throw);
            var binary = source.ToBase(2, options);
            var exact = binary.IsExactRepresentation;

            Assert.That(exact, Is.True);
            Assert.That(binary.Integral, Is.EqualTo(new[] { 1, 0, 1, 0 }));
            Assert.That(binary.Decimals, Is.EqualTo(new[] { 1, 0, 1 }));
            Assert.That(binary.ToDecimal(), Is.EqualTo(10.625m));
        }

        [Test]
        public void RepeatingFractionReportsTruncation()
        {
            var oneThird = NumeralValue.FromDigits(new[] { 0 }, new[] { 1 }, false, 3);

            var options = new NumeralConversionOptions(
                6,
                NumeralRoundingMode.ToZero,
                false,
                InfiniteExpansionBehavior.Truncate);
            var decimalValue = oneThird.ToBase(10, options);
            var exact = decimalValue.IsExactRepresentation;

            Assert.That(exact, Is.False);
            Assert.That(decimalValue.Decimals, Is.EqualTo(new[] { 3, 3, 3, 3, 3, 3 }));
            Assert.That(decimalValue.ExactValue, Is.EqualTo(new RationalValue(1, 3)));
        }

        [Test]
        public void ExplicitDecimalConversionReportsWhetherExpansionTerminates()
        {
            var terminating = BaseDecimal.ToIndicesOfBase(0.5m, 2, 8);
            var repeating = BaseDecimal.ToIndicesOfBase(0.1m, 2, 4);

            Assert.That(terminating.Exact, Is.True);
            Assert.That(terminating.Fractional, Is.EqualTo(new ulong[] { 1 }));
            Assert.That(repeating.Exact, Is.False);
            Assert.That(repeating.Fractional, Is.EqualTo(new ulong[] { 0, 0, 0, 1 }));
        }

        [Test]
        public void FractionalDigitLimitIsValidated()
        {
            var value = NumeralValue.FromDecimal(0.5m);

            Assert.Throws<ArgumentOutOfRangeException>(() => new NumeralConversionOptions(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => BaseDecimal.ToIndicesOfBase(0.5m, 2, -1));
        }

        [Test]
        public void BigIntegerRoundTripsAcrossNumeralApis()
        {
            var value = BigInteger.Pow(2, 256) + BigInteger.Parse("12345678901234567890");
            var hexadecimal = Numeral.System.OfBase(16);
            var ternary = Numeral.System.OfBase(3);
            hexadecimal.AdjustToFitIntegralLength = false;
            ternary.AdjustToFitIntegralLength = false;

            var numeral = hexadecimal[value];
            var converted = numeral.To(ternary, NumeralConversionOptions.Default);
            var digitValue = Value.FromBigInteger(value, 36);

            Assert.That(numeral.BigInteger, Is.EqualTo(value));
            Assert.That(converted.BigInteger, Is.EqualTo(value));
            Assert.That(digitValue.ToBigInteger(), Is.EqualTo(value));
            Assert.That(digitValue.ToBase(7, true).ToBigInteger(), Is.EqualTo(value));
        }

        [Test]
        public void IntegralBigIntegerConversionTruncatesFractionalDigits()
        {
            var result = BaseBigInteger.FromIndicesOfBase(
                new ulong[] { 1, 0 },
                new ulong[] { 1 },
                false,
                2);

            Assert.That(result, Is.EqualTo(new BigInteger(-2)));
        }

        [Test]
        public void SignedIntegralIndexersPreserveMinValues()
        {
            var decimalSystem = Numeral.System.OfBase(10);
            decimalSystem.AdjustToFitIntegralLength = false;

            Assert.That(decimalSystem[long.MinValue].BigInteger, Is.EqualTo(new BigInteger(long.MinValue)));
            Assert.That(decimalSystem[int.MinValue].BigInteger, Is.EqualTo(new BigInteger(int.MinValue)));
            Assert.That(decimalSystem[short.MinValue].BigInteger, Is.EqualTo(new BigInteger(short.MinValue)));
            Assert.That(decimalSystem[sbyte.MinValue].BigInteger, Is.EqualTo(new BigInteger(sbyte.MinValue)));
        }

        [Test]
        public void ImmutableReplacementUpdatesTheNumeralSign()
        {
            var numeral = Numeral.System.OfBase(10)[1];

            numeral = numeral.WithExactValue(RationalValue.FromInteger(-42));
            Assert.That(numeral.BigInteger, Is.EqualTo(new BigInteger(-42)));

            numeral = numeral.WithExactValue(RationalValue.FromInteger(BigInteger.Pow(-2, 65)));
            Assert.That(numeral.Positive, Is.False);
            Assert.That(numeral.BigInteger, Is.EqualTo(BigInteger.Pow(-2, 65)));
        }

        [Test]
        public void TextualIntegerConversionRespectsNegativeSign()
        {
            var decimalSystem = Numeral.System.OfBase(10);
            var identity = Enumerable.Range(0, 10).Select(x => x.ToString()).ToList();

            var success = decimalSystem.TryIntegerOf(
                "-42",
                identity,
                string.Empty,
                "-",
                ".",
                out var result);

            Assert.That(success, Is.True);
            Assert.That(result, Is.EqualTo(-42));
        }

        [Test]
        public void NumeralValueFromValuePreservesBaseAndDigits()
        {
            var source = new Value(new List<int> { 1, 0, 1 }, 2);

            var result = NumeralValue.FromValue(source);

            Assert.That(result.Base, Is.EqualTo(2));
            Assert.That(result.Integral, Is.EqualTo(source.Indices));
        }

        [Test]
        public void ValuePreservesLeadingZeroWidthWithoutAddingAnExtraZero()
        {
            var nonZero = new Value(new List<int> { 0, 0, 1 }, 10);
            var zero = new Value(new List<int> { 0, 0 }, 10);

            Assert.That(nonZero.ToBase(2).Indices, Is.EqualTo(new[] { 0, 0, 1 }));
            Assert.That(zero.ToBase(2).Indices, Is.EqualTo(new[] { 0, 0 }));
        }

        [Test]
        public void ValueRejectsNegativeBigInteger()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Value.FromBigInteger(-BigInteger.One));
        }

        [Test]
        public void NonFiniteFloatingPointValuesAreRejected()
        {
            var decimalSystem = Numeral.System.OfBase(10);

            Assert.Throws<ArgumentOutOfRangeException>(() => _ = decimalSystem[double.NaN]);
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = decimalSystem[double.PositiveInfinity]);
        }
    }
}
