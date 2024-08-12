using System;
using NumeralSystems.Net.Type.Base;
using NUnit.Framework;

namespace NumeralSystem.Net.NUnit.Utils
{
    [TestFixture]
    public class EncodeTests
    {
        private readonly Random _random = new();

        [Test]
        public void Uint()
        {
            var nBase = _random.Next(2, int.MaxValue);
            var value = _random.Next(int.MinValue, int.MaxValue);
            var encoded = UInt.ToIndicesOfBase(value, nBase, out var positive);
            var decoded = UInt.FromIndicesOfBase(encoded, nBase, positive);
            Assert.That(value, Is.EqualTo(decoded));
        }
        
        [Test]
        public void ULong()
        {
            var nBase = _random.Next(2, int.MaxValue);
            var value = (ulong)_random.Next(int.MinValue, int.MaxValue);
            var encoded = NumeralSystems.Net.Type.Base.ULong.ToIndicesOfBase(value, nBase);
            var decoded = NumeralSystems.Net.Type.Base.ULong.FromIndicesOfBase(encoded, nBase);
            Assert.That(value, Is.EqualTo(decoded).Within(0.000000000000001));
        }
        
        [Test(Author = "Mi", Description = "Failed 10045594282/job/27763245466")]
        public void ULongFailedTest0()
        {
            var nBase = _random.Next(2, int.MaxValue);
            var value = 18446744073499964511ul;
            var encoded = NumeralSystems.Net.Type.Base.ULong.ToIndicesOfBase(value, nBase);
            var decoded = NumeralSystems.Net.Type.Base.ULong.FromIndicesOfBase(encoded, nBase);
            Assert.That(value, Is.EqualTo(decoded).Within(0.0000000000000001));
        }
        
        [Test]
        public void Double()
        {
            var nBase = _random.Next(2, int.MaxValue);
            var value = _random.NextDouble() * _random.Next(int.MinValue, int.MaxValue);
            var encoded = NumeralSystems.Net.Type.Base.Double.ToIndicesOfBase(value, nBase);
            var decoded = NumeralSystems.Net.Type.Base.Double.FromIndicesOfBase(encoded.Integral, encoded.Fractional, encoded.positive, nBase);
            Assert.That(value, Is.EqualTo(decoded).Within(0.000000000000001));
        }
        
        [Test(Author = "Mi", Description = "Failed 10045594282/job/27763245466")]
        public void FloatFailedTest0()
        {
            var nBase = _random.Next(2, int.MaxValue);
            var value = 223346432.0f;
            var encoded = NumeralSystems.Net.Type.Base.Float.ToIndicesOfBase(value, nBase);
            var decoded = NumeralSystems.Net.Type.Base.Float.FromIndicesOfBase(encoded.Integral, encoded.Fractional, encoded.positive, nBase);
            Assert.That(value, Is.EqualTo(decoded).Within(0.000000000000001));
        }
        
        [Test]
        public void Float()
        {
            var nBase = _random.Next(2, int.MaxValue);
            var value = (float)(_random.NextDouble() * _random.Next(int.MinValue, int.MaxValue));
            var encoded = NumeralSystems.Net.Type.Base.Float.ToIndicesOfBase(value, nBase);
            var decoded = NumeralSystems.Net.Type.Base.Float.FromIndicesOfBase(encoded.Integral, encoded.Fractional, encoded.positive, nBase);
            Assert.That(value, Is.EqualTo(decoded).Within(0.000000000000001));
        }
        
        [Test]
        public void Decimal()
        {
            var nBase = _random.Next(2, int.MaxValue);
            var value = (decimal)(_random.NextDouble() * _random.Next(int.MinValue, int.MaxValue));
            var encoded = NumeralSystems.Net.Type.Base.Decimal.ToIndicesOfBase(value, nBase);
            var decoded = NumeralSystems.Net.Type.Base.Decimal.FromIndicesOfBase(encoded.Integral, encoded.Fractional, encoded.positive, nBase);
            Assert.That(value, Is.EqualTo(decoded).Within(0.000000000000001));
        }
        
        [Test]
        public void String()
        {
            var nBase = _random.Next(2, char.MaxValue);
            var testString = "Hello World!";
            var encoded = NumeralSystems.Net.Type.Base.String.ToIndicesOfBase(testString, nBase);
            var decoded = NumeralSystems.Net.Type.Base.String.FromIndicesOfBase(encoded, nBase);
            var e = NumeralSystems.Net.Type.Base.String.EncodeToBase(testString, nBase, out var size);
            var d = NumeralSystems.Net.Type.Base.String.DecodeFromBase(e, nBase, size);
            Assert.That(testString, Is.EqualTo(decoded));
            Assert.That(testString, Is.EqualTo(d));
        }
        
    }
}