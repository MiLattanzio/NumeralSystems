using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NumeralSystems.Net;
using NumeralSystems.Net.Type.Incomplete;
using NUnit.Framework;
using BaseByte = NumeralSystems.Net.Type.Base.Byte;
using BaseBigInteger = NumeralSystems.Net.Type.Base.BigInteger;
using BaseString = NumeralSystems.Net.Type.Base.String;
using BaseULong = NumeralSystems.Net.Type.Base.ULong;

namespace NumeralSystem.Net.NUnit
{
    [TestFixture]
    public class ValidationTests
    {
        [TestCase(0)]
        [TestCase(1)]
        public void PositionalTypesRejectBasesSmallerThanTwo(int baseValue)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new NumeralSystems.Net.NumeralSystem(baseValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Value(new List<int> { 0 }, baseValue));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new NumeralValue(new List<int> { 0 }, new List<int>(), false, baseValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => BaseULong.ToIndicesOfBase(10, baseValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => BaseBigInteger.ToIndicesOfBase(10, baseValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => BaseString.EncodeToBase("value", baseValue, out _));
        }

        [Test]
        public void ValuesRejectDigitsOutsideTheirBase()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Value(new List<int> { -1 }, 10));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Value(new List<int> { 10 }, 10));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new NumeralValue(new List<int> { -1 }, new List<int>(), false, 10));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new NumeralValue(new List<int> { 0 }, new List<int> { 10 }, false, 10));
            Assert.Throws<ArgumentOutOfRangeException>(() => BaseULong.FromIndicesOfBase(new ulong[] { 2 }, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => BaseBigInteger.FromIndicesOfBase(new ulong[] { 16 }, 16));
        }

        [Test]
        public void ToValuePreservesBaseAndIntegralDigits()
        {
            var numeralValue = new NumeralValue(new List<int> { 1, 0, 1 }, new List<int> { 1 }, false, 2);

            var value = numeralValue.ToValue();

            Assert.That(value.Base, Is.EqualTo(2));
            Assert.That(value.Indices, Is.EqualTo(new[] { 1, 0, 1 }));
        }

        [Test]
        public void NegativeBigIntegerUsesMagnitudeDigitsAndPreservesSign()
        {
            var value = NumeralValue.FromBigInteger(new BigInteger(-255));

            Assert.That(value.Negative, Is.True);
            Assert.That(value.Integral, Is.EqualTo(new[] { 2, 5, 5 }));
            Assert.That(value.ToBigInteger(), Is.EqualTo(new BigInteger(-255)));
        }

        [Test]
        public void IncompleteByteIndexerMapsPermutationBitsInOrder()
        {
            var value = new BaseByte { Value = 2 }.Incomplete();
            value.Binary[0] = null;
            value.Binary[2] = null;

            var candidates = Enumerable.Range(0, 4)
                .Select(index => value[(uint)index].Value)
                .ToArray();

            Assert.That(candidates, Is.EqualTo(new byte[] { 2, 3, 6, 7 }));
        }
    }
}
