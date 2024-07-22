using System;
using NUnit.Framework;
using Int32 = NumeralSystems.Net.Type.Base.Int32;

namespace NumeralSystem.Net.NUnit
{
    [TestFixture]
    public class BinaryOperationsTests
    {
        public Random Random = new ();
      
        [Test]
        public void AndTest()
        {
            var numberA = Random.Next(0, int.MaxValue);
            var numberB = Random.Next(0, int.MaxValue);
            var a = new Int32() { Value = numberA };
            var b = new Int32() { Value = numberB };
            var numberC = numberA & numberB;
            var c = a.And(b);
            Assert.That(numberC, Is.EqualTo(c.Value));
        }
        
        [Test]
        public void OrTest()
        {
            var numberA = Random.Next(0, int.MaxValue);
            var numberB = Random.Next(0, int.MaxValue);
            var a = new Int32() { Value = numberA };
            var b = new Int32() { Value = numberB };
            var numberC = numberA | numberB;
            var c = a.Or(b);
            Assert.That(numberC, Is.EqualTo(c.Value));
        }
    }
}