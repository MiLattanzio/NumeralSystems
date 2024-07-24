using System;
using NumeralSystems.Net.Type.Base;
using NUnit.Framework;

namespace NumeralSystem.Net.NUnit.Math
{
    [TestFixture]
    public class Bitwise
    {
        Random _random = new Random();
        [Test]
        public void And()
        {
            var int32A = new Int()
            {
                Value = _random.Next()
            };
            var int32B = new Int()
            {
                Value = _random.Next()
            };
            Assert.That(int32A.Value & int32B.Value, Is.EqualTo(int32A.And(int32B).Value));
        }
        
        [Test]
        public void Or()
        {
            var int32A = new Int()
            {
                Value = _random.Next()
            };
            var int32B = new Int()
            {
                Value = _random.Next()
            };
            Assert.That(int32A.Value | int32B.Value, Is.EqualTo(int32A.Or(int32B).Value));
        }
        
        [Test]
        public void Xor()
        {
            var int32A = new Int()
            {
                Value = _random.Next()
            };
            var int32B = new Int()
            {
                Value = _random.Next()
            };
            Assert.That(int32A.Value ^ int32B.Value, Is.EqualTo(int32A.Xor(int32B).Value));
        }
        
        [Test]
        public void Not()
        {
            var int32 = new Int()
            {
                Value = _random.Next()
            };
            Assert.That(~int32.Value, Is.EqualTo(int32.Not().Value));
        }
        
        [Test]
        public void Nand()
        {
            var int32A = new Int()
            {
                Value = _random.Next()
            };
            var int32B = new Int()
            {
                Value = _random.Next()
            };
            Assert.That(~(int32A.Value & int32B.Value), Is.EqualTo(int32A.Nand(int32B).Value));
        }
    }
}