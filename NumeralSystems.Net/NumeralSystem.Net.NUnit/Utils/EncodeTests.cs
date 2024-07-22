using System;
using NUnit.Framework;

namespace NumeralSystem.Net.NUnit.Utils
{
    [TestFixture]
    public class EncodeTests
    {
        private readonly Random _random = new Random();

        [Test]
        public void Uint()
        {
            var nBase = _random.Next(2, int.MaxValue);
            var value = _random.Next(int.MinValue, int.MaxValue);
            var encoded = NumeralSystems.Net.Utils.Encode.UInt.ToIndicesOfBase(value, nBase, out var positive);
            var decoded = NumeralSystems.Net.Utils.Encode.UInt.FromIndicesOfBase(encoded, nBase, positive);
            Assert.That(value, Is.EqualTo(decoded));
        }
        
        [Test]
        public void ULong()
        {
            var nBase = _random.Next(2, int.MaxValue);
            var value = (ulong)_random.Next(int.MinValue, int.MaxValue);
            var encoded = NumeralSystems.Net.Utils.Encode.ULong.ToIndicesOfBase(value, nBase);
            var decoded = NumeralSystems.Net.Utils.Encode.ULong.FromIndicesOfBase(encoded, nBase);
            Assert.That(value, Is.EqualTo(decoded).Within(0.000000000000001));
        }
        
        [Test(Author = "Mi", Description = "Failed 10045594282/job/27763245466")]
        public void ULongFailedTest0()
        {
            var nBase = _random.Next(2, int.MaxValue);
            var value = 18446744073499964511ul;
            var encoded = NumeralSystems.Net.Utils.Encode.ULong.ToIndicesOfBase(value, nBase);
            var decoded = NumeralSystems.Net.Utils.Encode.ULong.FromIndicesOfBase(encoded, nBase);
            Assert.That(value, Is.EqualTo(decoded).Within(0.0000000000000001));
        }
        
        [Test]
        public void Double()
        {
            var nBase = _random.Next(2, int.MaxValue);
            var value = _random.NextDouble() * _random.Next(int.MinValue, int.MaxValue);
            var encoded = NumeralSystems.Net.Utils.Encode.Double.ToIndicesOfBase(value, nBase);
            var decoded = NumeralSystems.Net.Utils.Encode.Double.FromIndicesOfBase(encoded.Integral, encoded.Fractional, encoded.positive, nBase);
            Assert.That(value, Is.EqualTo(decoded).Within(0.000000000000001));
        }
        
        [Test]
        public void Float()
        {
            var nBase = _random.Next(2, int.MaxValue);
            var value = (float)(_random.NextDouble() * _random.Next(int.MinValue, int.MaxValue));
            var encoded = NumeralSystems.Net.Utils.Encode.Float.ToIndicesOfBase(value, nBase);
            var decoded = NumeralSystems.Net.Utils.Encode.Float.FromIndicesOfBase(encoded.Integral, encoded.Fractional, encoded.positive, nBase);
            Assert.That(value, Is.EqualTo(decoded).Within(0.000000000000001));
        }
        
        [Test]
        public void Decimal()
        {
            var nBase = _random.Next(2, int.MaxValue);
            var value = (decimal)(_random.NextDouble() * _random.Next(int.MinValue, int.MaxValue));
            var encoded = NumeralSystems.Net.Utils.Encode.Decimal.ToIndicesOfBase(value, nBase);
            var decoded = NumeralSystems.Net.Utils.Encode.Decimal.FromIndicesOfBase(encoded.Integral, encoded.Fractional, encoded.positive, nBase);
            Assert.That(value, Is.EqualTo(decoded).Within(0.000000000000001));
        }
        
        [Test]
        public void String()
        {
            var nBase = _random.Next(2, char.MaxValue);
            var testString = "Hello World!";
            var encoded = NumeralSystems.Net.Utils.Encode.String.ToIndicesOfBase(testString, nBase);
            var decoded = NumeralSystems.Net.Utils.Encode.String.FromIndicesOfBase(encoded, nBase);
            var e = NumeralSystems.Net.Utils.Encode.String.EncodeToBase(testString, nBase, out var size);
            var d = NumeralSystems.Net.Utils.Encode.String.DecodeFromBase(e, nBase, size);
            Assert.That(testString, Is.EqualTo(decoded));
            Assert.That(testString, Is.EqualTo(d));
        }
        
    }
}