using System;
using NUnit.Framework;

namespace NumeralSystem.Net.NUnit.Type
{
    [TestFixture]
    public class Int
    {
        Random _random = new Random();

        [Test]
        public void Generation()
        {
            var number = _random.Next();
            var int32 = new NumeralSystems.Net.Type.Base.Int(){ Value = number };
            Assert.That(number, Is.EqualTo(int32.Value));
            var fromBinary = new NumeralSystems.Net.Type.Base.Int()
            {
                Binary = int32.Binary
            };
            Assert.That(number, Is.EqualTo(fromBinary.Value));
            var fromBytes = new NumeralSystems.Net.Type.Base.Int()
            {
                Bytes = int32.Bytes
            };
            Assert.That(number, Is.EqualTo(fromBytes.Value));
        }
        
        [Test]
        public void And()
        {
            var int32A = new NumeralSystems.Net.Type.Base.Int()
            {
                Value = _random.Next()
            };
            var int32B = new NumeralSystems.Net.Type.Base.Int()
            {
                Value = _random.Next()
            };
            Assert.That(int32A.Value & int32B.Value, Is.EqualTo(int32A.And(int32B).Value));
        }
        
        [Test]
        public void Or()
        {
            var int32A = new NumeralSystems.Net.Type.Base.Int()
            {
                Value = _random.Next()
            };
            var int32B = new NumeralSystems.Net.Type.Base.Int()
            {
                Value = _random.Next()
            };
            Assert.That(int32A.Value | int32B.Value, Is.EqualTo(int32A.Or(int32B).Value));
        }
        
        [Test]
        public void Xor()
        {
            var int32A = new NumeralSystems.Net.Type.Base.Int()
            {
                Value = _random.Next()
            };
            var int32B = new NumeralSystems.Net.Type.Base.Int()
            {
                Value = _random.Next()
            };
            Assert.That(int32A.Value ^ int32B.Value, Is.EqualTo(int32A.Xor(int32B).Value));
        }
        
        [Test]
        public void Not()
        {
            var int32 = new NumeralSystems.Net.Type.Base.Int()
            {
                Value = _random.Next()
            };
            Assert.That(~int32.Value, Is.EqualTo(int32.Not().Value));
        }
        
        [Test]
        public void Nand()
        {
            var int32A = new NumeralSystems.Net.Type.Base.Int()
            {
                Value = _random.Next()
            };
            var int32B = new NumeralSystems.Net.Type.Base.Int()
            {
                Value = _random.Next()
            };
            Assert.That(~(int32A.Value & int32B.Value), Is.EqualTo(int32A.Nand(int32B).Value));
        }
        
        [Test]
        public void ReverseAnd()
        {
            var int32A = new NumeralSystems.Net.Type.Base.Int()
            {
                Value = _random.Next()
            };
            var int32B = new NumeralSystems.Net.Type.Base.Int()
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
            var int32A = new NumeralSystems.Net.Type.Base.Int()
            {
                Value = _random.Next()
            };
            var int32B = new NumeralSystems.Net.Type.Base.Int()
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