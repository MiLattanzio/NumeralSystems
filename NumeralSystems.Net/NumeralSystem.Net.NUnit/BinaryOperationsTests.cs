using System;
using NumeralSystems.Net.Type.Base;
using NUnit.Framework;

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
            var a = new Int() { Value = numberA };
            var b = new Int() { Value = numberB };
            var numberC = numberA & numberB;
            var c = a.And(b);
            Assert.That(numberC, Is.EqualTo(c.Value));
        }
        
        [Test]
        public void OrTest()
        {
            var numberA = Random.Next(0, int.MaxValue);
            var numberB = Random.Next(0, int.MaxValue);
            var a = new Int() { Value = numberA };
            var b = new Int() { Value = numberB };
            var numberC = numberA | numberB;
            var c = a.Or(b);
            Assert.That(numberC, Is.EqualTo(c.Value));
        }
    }
}