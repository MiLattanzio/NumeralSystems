using System;
using NumeralSystems.Net.Type.Base;
using NUnit.Framework;

namespace NumeralSystem.Net.NUnit
{
    [TestFixture]
    public class ReverseBinaryOperationsTests
    {
        private Random _random;

        [SetUp]
        public void Setup()
        {
            _random = new Random(42);
        }
      
        [Test]
        public void ReverseAndTest()
        {
            var numberA = _random.Next(0, int.MaxValue);
            var numberB = _random.Next(0, int.MaxValue);
            var a = new Int() { Value = numberA };
            var b = new Int() { Value = numberB };
            var c = a.And(b);
            var success = c.ReverseAnd(b, out var reversedA);
            Assert.That(success, Is.True);
            Assert.IsTrue(reversedA.Contains(a));
        }
        
        [Test]
        public void ReverseOrTest()
        {
            var numberA = _random.Next(0, int.MaxValue);
            var numberB = _random.Next(0, int.MaxValue);
            var a = new Int() { Value = numberA };
            var b = new Int() { Value = numberB };
            var c = a.Or(b);
            var success = c.ReverseOr(b, out var reversedA);
            Assert.That(success, Is.True);
            Assert.IsTrue(reversedA.Contains(a));
        }
    }
}