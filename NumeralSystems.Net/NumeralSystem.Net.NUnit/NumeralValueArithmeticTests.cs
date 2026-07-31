using System;
using System.Collections.Generic;
using System.Numerics;
using NumeralSystems.Net;
using NUnit.Framework;

namespace NumeralSystem.Net.NUnit
{
    [TestFixture]
    public class NumeralValueArithmeticTests
    {
        [Test]
        public void AddUsesExactRationalArithmeticWithoutMutatingOperands()
        {
            var left = ValueOf(new[] { 1, 0 }, new[] { 5 }, baseValue: 10);
            var right = ValueOf(new[] { 2 }, new[] { 2, 5 }, baseValue: 10);

            var result = left.Add(right, out var exact);

            Assert.That(exact, Is.True);
            Assert.That(result.Base, Is.EqualTo(10));
            Assert.That(result.Integral, Is.EqualTo(new[] { 1, 2 }));
            Assert.That(result.Decimals, Is.EqualTo(new[] { 7, 5 }));
            Assert.That(left.ToDecimal(), Is.EqualTo(10.5m));
            Assert.That(right.ToDecimal(), Is.EqualTo(2.25m));
        }

        [Test]
        public void AddSupportsOperandsWithDifferentBases()
        {
            var oneHalf = ValueOf(new[] { 0 }, new[] { 1 }, baseValue: 2);
            var oneQuarter = ValueOf(new[] { 0 }, new[] { 1 }, baseValue: 4);

            var result = oneHalf.Add(oneQuarter, out var exact);

            Assert.That(exact, Is.True);
            Assert.That(result.Base, Is.EqualTo(2));
            Assert.That(result.Integral, Is.EqualTo(new[] { 0 }));
            Assert.That(result.Decimals, Is.EqualTo(new[] { 1, 1 }));
            Assert.That(result.ToDecimal(), Is.EqualTo(0.75m));
        }

        [Test]
        public void SubtractCanProduceANegativeResult()
        {
            var one = NumeralValue.FromInt(1);
            var oneAndOneHalf = NumeralValue.FromDecimal(1.5m);

            var result = one.Subtract(oneAndOneHalf, out var exact);

            Assert.That(exact, Is.True);
            Assert.That(result.Negative, Is.True);
            Assert.That(result.ToDecimal(), Is.EqualTo(-0.5m));
        }

        [Test]
        public void MultiplyPreservesSignAndFraction()
        {
            var negativeOneAndOneHalf = NumeralValue.FromDecimal(-1.5m);
            var two = NumeralValue.FromInt(2);

            var result = negativeOneAndOneHalf.Multiply(two, out var exact);

            Assert.That(exact, Is.True);
            Assert.That(result.Negative, Is.True);
            Assert.That(result.ToDecimal(), Is.EqualTo(-3m));
        }

        [Test]
        public void DivideCanChooseAResultBaseWhereTheFractionTerminates()
        {
            var one = NumeralValue.FromInt(1);
            var four = NumeralValue.FromInt(4);

            var result = one.Divide(
                four,
                out var exact,
                resultBase: 2,
                maxFractionalDigits: 8);

            Assert.That(exact, Is.True);
            Assert.That(result.Base, Is.EqualTo(2));
            Assert.That(result.Integral, Is.EqualTo(new[] { 0 }));
            Assert.That(result.Decimals, Is.EqualTo(new[] { 0, 1 }));
        }

        [Test]
        public void DivideReportsARepeatingExpansion()
        {
            var one = NumeralValue.FromInt(1);
            var three = NumeralValue.FromInt(3);

            var result = one.Divide(
                three,
                out var exact,
                resultBase: 10,
                maxFractionalDigits: 6);

            Assert.That(exact, Is.False);
            Assert.That(result.Decimals, Is.EqualTo(new[] { 3, 3, 3, 3, 3, 3 }));
            Assert.That(result.ToDecimal(), Is.EqualTo(0.333333m));
        }

        [Test]
        public void ArithmeticOperatorsUseTheLeftOperandBase()
        {
            var oneHalfBinary = ValueOf(new[] { 0 }, new[] { 1 }, baseValue: 2);
            var oneHalfDecimal = NumeralValue.FromDecimal(0.5m);

            var sum = oneHalfBinary + oneHalfDecimal;
            var difference = oneHalfBinary - oneHalfDecimal;
            var product = oneHalfBinary * oneHalfDecimal;
            var quotient = oneHalfBinary / oneHalfDecimal;

            Assert.That(sum.Base, Is.EqualTo(2));
            Assert.That(sum.ToDecimal(), Is.EqualTo(1m));
            Assert.That(difference.IsZero, Is.True);
            Assert.That(product.ToDecimal(), Is.EqualTo(0.25m));
            Assert.That(quotient.ToDecimal(), Is.EqualTo(1m));
        }

        [Test]
        public void ComparisonUsesNumericValueInsteadOfRepresentation()
        {
            var oneHalfBinary = ValueOf(new[] { 0 }, new[] { 1 }, baseValue: 2);
            var oneHalfDecimal = NumeralValue.FromDecimal(0.5m);
            var negative = NumeralValue.FromInt(-1);

            Assert.That(oneHalfBinary.NumericallyEquals(oneHalfDecimal), Is.True);
            Assert.That(oneHalfBinary.CompareTo(oneHalfDecimal), Is.Zero);
            Assert.That(oneHalfBinary <= oneHalfDecimal, Is.True);
            Assert.That(oneHalfBinary >= oneHalfDecimal, Is.True);
            Assert.That(oneHalfBinary < oneHalfDecimal, Is.False);
            Assert.That(negative < oneHalfBinary, Is.True);
        }

        [Test]
        public void ArithmeticDoesNotOverflowPrimitiveIntegerRanges()
        {
            var magnitude = BigInteger.Pow(2, 300);
            var left = NumeralValue.FromBigInteger(magnitude, 16);
            var right = NumeralValue.FromBigInteger(magnitude, 3);

            var sum = left + right;
            var product = left * NumeralValue.FromInt(8);

            Assert.That(sum.ToBigInteger(), Is.EqualTo(magnitude * 2));
            Assert.That(product.ToBigInteger(), Is.EqualTo(magnitude * 8));
        }

        [Test]
        public void NegateAndAbsNormalizeZeroSign()
        {
            var negative = NumeralValue.FromDecimal(-12.5m);
            var zero = NumeralValue.FromInt(0);

            Assert.That(negative.Abs().ToDecimal(), Is.EqualTo(12.5m));
            Assert.That(negative.Negate().ToDecimal(), Is.EqualTo(12.5m));
            Assert.That((-negative).ToDecimal(), Is.EqualTo(12.5m));
            Assert.That(zero.Negate().Negative, Is.False);
        }

        [Test]
        public void DivideByZeroAndNullOperandsAreRejected()
        {
            var one = NumeralValue.FromInt(1);
            var zero = NumeralValue.FromInt(0);

            Assert.Throws<DivideByZeroException>(() => one.Divide(zero));
            Assert.Throws<ArgumentNullException>(() => one.Add(null));
            Assert.Throws<ArgumentNullException>(() => _ = one + null);
        }

        [Test]
        public void ArithmeticValidatesResultBaseAndPrecision()
        {
            var one = NumeralValue.FromInt(1);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                one.Add(one, out _, resultBase: 1));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                one.Multiply(one, out _, maxFractionalDigits: -1));
        }

        private static NumeralValue ValueOf(
            IEnumerable<int> integral,
            IEnumerable<int> fractional,
            bool negative = false,
            int baseValue = 10)
            => new NumeralValue(
                new List<int>(integral),
                new List<int>(fractional),
                negative,
                baseValue);
    }
}
