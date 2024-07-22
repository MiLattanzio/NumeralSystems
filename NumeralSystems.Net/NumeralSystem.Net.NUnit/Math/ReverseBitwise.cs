using System;
using NUnit.Framework;
using Int32 = NumeralSystems.Net.Type.Base.Int32;

namespace NumeralSystem.Net.NUnit.Math
{
    [TestFixture]
    public class ReverseBitwise
    {
        Random _random = new Random();
        
        [Test]
        public void ReverseAnd()
        {
            var int32A = new Int32()
            {
                Value = _random.Next()
            };
            var int32B = new Int32()
            {
                Value = _random.Next()
            };
            var result = int32A.And(int32B);
            var success = result.ReverseAnd(int32B, out var int32AReversed);
            Assert.IsTrue(success);
            Assert.IsTrue(int32AReversed.Contains(int32A));
        }
        
        [Test]
        public void ReverseOr()
        {
            var int32A = new Int32()
            {
                Value = _random.Next()
            };
            var int32B = new Int32()
            {
                Value = _random.Next()
            };
            var result = int32A.Or(int32B);
            var success = result.ReverseOr(int32B, out var int32AReversed);
            Assert.IsTrue(success);
            Assert.IsTrue(int32AReversed.Contains(int32A));
        }
    }
}