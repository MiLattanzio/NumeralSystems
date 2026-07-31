using System;
using System.Collections.Generic;
using System.Numerics;
using NumeralSystems.Net;
using NUnit.Framework;

namespace NumeralSystem.Net.NUnit
{
    [TestFixture]
    public class RationalValueTests
    {
        [Test]
        public void RationalValuesAreNormalizedAndImmutable()
        {
            var value = new RationalValue(-6, -8);

            Assert.That(value.Numerator, Is.EqualTo(new BigInteger(3)));
            Assert.That(value.Denominator, Is.EqualTo(new BigInteger(4)));
            Assert.That(new RationalValue(0, 99), Is.EqualTo(RationalValue.Zero));
            Assert.That(typeof(RationalValue).IsSealed, Is.True);
            Assert.Throws<DivideByZeroException>(() => new RationalValue(1, 0));
        }

        [Test]
        public void DecimalConversionHandlesLargeRatioComponents()
        {
            var denominator = BigInteger.Pow(12, 128);
            var value = new RationalValue(denominator * 1_000_000_000 + 1, denominator);

            Assert.That(value.ToDecimal(), Is.EqualTo(1_000_000_000m));
        }

        [Test]
        public void FloatingPointFactoriesPreserveTheIeeeValueExactly()
        {
            var value = RationalValue.FromDouble(0.1d);

            Assert.That(value, Is.Not.EqualTo(new RationalValue(1, 10)));
            Assert.That((double)value.Numerator / (double)value.Denominator, Is.EqualTo(0.1d));
            Assert.Throws<ArgumentOutOfRangeException>(() => RationalValue.FromDouble(double.NaN));
        }

        [Test]
        public void RequestedTerminatingExamplesAreExact()
        {
            var decimalHalf = NumeralValue.FromRational(1, 2, 10);
            var binaryHalf = decimalHalf.ToBase(2, NumeralConversionOptions.Default);
            var ternaryThird = NumeralValue.FromRational(1, 3, 3);

            Assert.That(decimalHalf.Integral, Is.EqualTo(new[] { 0 }));
            Assert.That(decimalHalf.Decimals, Is.EqualTo(new[] { 5 }));
            Assert.That(binaryHalf.Decimals, Is.EqualTo(new[] { 1 }));
            Assert.That(binaryHalf.ExactValue, Is.EqualTo(decimalHalf.ExactValue));
            Assert.That(ternaryThird.Decimals, Is.EqualTo(new[] { 1 }));
            Assert.That(ternaryThird.IsExactRepresentation, Is.True);
        }

        [Test]
        public void DecimalOneTenthDetectsItsBinaryPrefixAndPeriod()
        {
            var value = NumeralValue.FromDecimal(0.1m);
            var expansion = value.Expand(2);
            var binary = value.ToBase(2, NumeralConversionOptions.Default);

            Assert.That(expansion.IsTerminating, Is.False);
            Assert.That(expansion.IsExact, Is.True);
            Assert.That(expansion.RepeatingStartIndex, Is.EqualTo(1));
            Assert.That(expansion.RepeatingLength, Is.EqualTo(4));
            Assert.That(expansion.FractionalDigits, Is.EqualTo(new[] { 0, 0, 0, 1, 1 }));
            Assert.That(expansion.ToString(NumeralAlphabet.Base2), Is.EqualTo("0.0(0011)"));
            Assert.That(binary.ExactValue, Is.EqualTo(new RationalValue(1, 10)));
        }

        [Test]
        public void TruncatedRepresentationKeepsTheExactRationalValue()
        {
            var truncate = new NumeralConversionOptions(
                6,
                NumeralRoundingMode.ToZero,
                false,
                InfiniteExpansionBehavior.Truncate);
            var oneThird = NumeralValue.FromRational(1, 3, 10, truncate);

            Assert.That(oneThird.Decimals, Is.EqualTo(new[] { 3, 3, 3, 3, 3, 3 }));
            Assert.That(oneThird.IsExactRepresentation, Is.False);
            Assert.That(oneThird.Numerator, Is.EqualTo(BigInteger.One));
            Assert.That(oneThird.Denominator, Is.EqualTo(new BigInteger(3)));

            var ternary = oneThird.ToBase(3, NumeralConversionOptions.Default);
            Assert.That(ternary.Decimals, Is.EqualTo(new[] { 1 }));
            Assert.That(ternary.IsExactRepresentation, Is.True);
        }

        [Test]
        public void GeneratedCrossBaseConversionsNeverChangeTheExactRatio()
        {
            var bounded = new NumeralConversionOptions(
                32,
                NumeralRoundingMode.ToNearestEven,
                false,
                InfiniteExpansionBehavior.Round);

            for (var numerator = -12; numerator <= 12; numerator++)
            for (var denominator = 1; denominator <= 12; denominator++)
            for (var baseValue = 2; baseValue <= 16; baseValue++)
            {
                var exact = new RationalValue(numerator, denominator);
                var converted = NumeralValue.FromRational(exact, baseValue, bounded);

                Assert.That(converted.ExactValue, Is.EqualTo(exact));
            }
        }

        [Test]
        public void RoundingModesHandleMidpointsDirectionAndCarry()
        {
            NumeralConversionOptions Round(NumeralRoundingMode mode, int digits = 1) =>
                new NumeralConversionOptions(digits, mode, false, InfiniteExpansionBehavior.Round);

            var even = NumeralValue.FromRational(1, 4, 10, Round(NumeralRoundingMode.ToNearestEven));
            var away = NumeralValue.FromRational(1, 4, 10, Round(NumeralRoundingMode.ToNearestAwayFromZero));
            var positive = NumeralValue.FromRational(1, 101, 10, Round(NumeralRoundingMode.ToPositiveInfinity, 2));
            var negative = NumeralValue.FromRational(-1, 101, 10, Round(NumeralRoundingMode.ToNegativeInfinity, 2));
            var carry = NumeralValue.FromRational(999, 1000, 10, Round(NumeralRoundingMode.ToNearestAwayFromZero, 2));

            Assert.That(even.Decimals, Is.EqualTo(new[] { 2 }));
            Assert.That(away.Decimals, Is.EqualTo(new[] { 3 }));
            Assert.That(positive.Decimals, Is.EqualTo(new[] { 0, 1 }));
            Assert.That(negative.Decimals, Is.EqualTo(new[] { 0, 1 }));
            Assert.That(negative.Negative, Is.True);
            Assert.That(carry.Integral, Is.EqualTo(new[] { 1 }));
            Assert.That(carry.Decimals, Is.EqualTo(new[] { 0, 0 }));
            Assert.That(carry.ExactValue, Is.EqualTo(new RationalValue(999, 1000)));
        }

        [Test]
        public void InfiniteExpansionPoliciesFailPredictably()
        {
            var throwOptions = new NumeralConversionOptions(
                16,
                NumeralRoundingMode.ToNearestEven,
                true,
                InfiniteExpansionBehavior.Throw);
            var shortPeriodLimit = new NumeralConversionOptions(
                5,
                NumeralRoundingMode.ToNearestEven,
                true,
                InfiniteExpansionBehavior.PreservePeriod);

            Assert.Throws<InfiniteNumeralExpansionException>(() =>
                NumeralValue.FromRational(1, 3, 10, throwOptions));
            var error = Assert.Throws<NumeralExpansionLimitException>(() =>
                NumeralValue.FromRational(1, 7, 10, shortPeriodLimit));
            Assert.That(error?.MaxFractionalDigits, Is.EqualTo(5));
            Assert.Throws<ArgumentException>(() => new NumeralConversionOptions(
                10,
                NumeralRoundingMode.ToZero,
                false,
                InfiniteExpansionBehavior.PreservePeriod));
        }

        [Test]
        public void TerminatingExpansionBeyondTheLimitIsNotReportedAsInfinite()
        {
            var options = new NumeralConversionOptions(
                10,
                NumeralRoundingMode.ToNearestEven,
                false,
                InfiniteExpansionBehavior.Throw);

            Assert.Throws<NumeralExpansionLimitException>(() =>
                NumeralValue.FromRational(1, BigInteger.One << 20, 2, options));
        }

        [Test]
        public void DigitCollectionsAndOptionsCannotBeMutated()
        {
            var value = NumeralValue.FromRational(1, 2, 10);
            var digits = (IList<int>)value.Decimals;
            var changed = NumeralConversionOptions.Default.WithMaxFractionalDigits(12);

            Assert.Throws<NotSupportedException>(() => digits[0] = 9);
            Assert.That(NumeralConversionOptions.Default.MaxFractionalDigits, Is.EqualTo(128));
            Assert.That(changed.MaxFractionalDigits, Is.EqualTo(12));
            Assert.That(typeof(NumeralValue).IsSealed, Is.True);
            Assert.That(typeof(NumeralConversionOptions).IsSealed, Is.True);
        }

        [Test]
        public void NumeralExposesAnExactSnapshotAndConvertsWithoutDecimal()
        {
            var binary = new Numeral(
                Numeral.System.OfBase(2),
                new List<int> { 0 },
                new List<int> { 1 });
            var decimalSystem = Numeral.System.OfBase(10);
            decimalSystem.AdjustToFitIntegralLength = false;

            var converted = binary.To(decimalSystem, NumeralConversionOptions.Default);

            Assert.That(binary.ExactValue, Is.EqualTo(new RationalValue(1, 2)));
            Assert.That(converted.IntegralIndices, Is.EqualTo(new[] { 0 }));
            Assert.That(converted.FractionalIndices, Is.EqualTo(new[] { 5 }));
        }
    }
}
