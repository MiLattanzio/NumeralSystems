using System;
using NUnit.Framework;
using Double = NumeralSystems.Net.Utils.Encode.Double;
using Int32 = NumeralSystems.Net.Type.Base.Int32;

namespace NumeralSystem.Net.NUnit.Math
{
    [TestFixture]
    public class Bitwise
    {
        Random _random = new Random();
        [Test]
        public void And()
        {
            var int32A = new Int32()
            {
                Value = _random.Next()
            };
            var int32B = new Int32()
            {
                Value = _random.Next()
            };
            Assert.That(int32A.Value & int32B.Value, Is.EqualTo(int32A.And(int32B).Value));
        }
        
        [Test]
        public void Or()
        {
            var int32A = new Int32()
            {
                Value = _random.Next()
            };
            var int32B = new Int32()
            {
                Value = _random.Next()
            };
            Assert.That(int32A.Value | int32B.Value, Is.EqualTo(int32A.Or(int32B).Value));
        }
        
        [Test]
        public void Xor()
        {
            var int32A = new Int32()
            {
                Value = _random.Next()
            };
            var int32B = new Int32()
            {
                Value = _random.Next()
            };
            Assert.That(int32A.Value ^ int32B.Value, Is.EqualTo(int32A.Xor(int32B).Value));
        }
        
        [Test]
        public void Not()
        {
            var int32 = new Int32()
            {
                Value = _random.Next()
            };
            Assert.That(~int32.Value, Is.EqualTo(int32.Not().Value));
        }
        
        [Test]
        public void Nand()
        {
            var int32A = new Int32()
            {
                Value = _random.Next()
            };
            var int32B = new Int32()
            {
                Value = _random.Next()
            };
            Assert.That(~(int32A.Value & int32B.Value), Is.EqualTo(int32A.Nand(int32B).Value));
        }
    }
}