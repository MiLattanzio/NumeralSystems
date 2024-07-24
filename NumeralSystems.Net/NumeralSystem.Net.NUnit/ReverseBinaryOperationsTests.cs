using System;
using NumeralSystems.Net.Type.Base;
using NUnit.Framework;

namespace NumeralSystem.Net.NUnit
{
    [TestFixture]
    public class ReverseBinaryOperationsTests
    {
        public Random Random = new ();
      
        [Test]
        public void ReverseAndTest()
        {
            var numberA = Random.Next(0, int.MaxValue);
            var numberB = Random.Next(0, int.MaxValue);
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
            var numberA = Random.Next(0, int.MaxValue);
            var numberB = Random.Next(0, int.MaxValue);
            var a = new Int() { Value = numberA };
            var b = new Int() { Value = numberB };
            var c = a.Or(b);
            var success = c.ReverseOr(b, out var reversedA);
            Assert.That(success, Is.True);
            Assert.IsTrue(reversedA.Contains(a));
        }
    }
}