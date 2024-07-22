using System;
using NUnit.Framework;
using Int32 = NumeralSystems.Net.Type.Base.Int32;

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
            var a = new Int32() { Value = numberA };
            var b = new Int32() { Value = numberB };
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
            var a = new Int32() { Value = numberA };
            var b = new Int32() { Value = numberB };
            var c = a.Or(b);
            var success = c.ReverseOr(b, out var reversedA);
            Assert.That(success, Is.True);
            Assert.IsTrue(reversedA.Contains(a));
        }
    }
}