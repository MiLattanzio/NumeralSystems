using System;
using NumeralSystems.Net.Type.Base;
using NUnit.Framework;

namespace NumeralSystem.Net.NUnit
{
    [TestFixture]
    public class BinaryOperationsTests
    {
        private Random _random;

        [SetUp]
        public void Setup()
        {
            _random = new Random(42);
        }
      
        [Test]
        public void AndTest()
        {
            var numberA = _random.Next(0, int.MaxValue);
            var numberB = _random.Next(0, int.MaxValue);
            var a = new Int() { Value = numberA };
            var b = new Int() { Value = numberB };
            var numberC = numberA & numberB;
            var c = a.And(b);
            Assert.That(numberC, Is.EqualTo(c.Value));
        }
        
        [Test]
        public void OrTest()
        {
            var numberA = _random.Next(0, int.MaxValue);
            var numberB = _random.Next(0, int.MaxValue);
            var a = new Int() { Value = numberA };
            var b = new Int() { Value = numberB };
            var numberC = numberA | numberB;
            var c = a.Or(b);
            Assert.That(numberC, Is.EqualTo(c.Value));
        }
    }
}