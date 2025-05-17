using System;
using NumeralSystems.Net.Utils;
using NUnit.Framework;
using Polecola.Primitive;

namespace NumeralSystem.Net.NUnit.Type
{
    [TestFixture]
    public class Long : IBaseTypeTest
    {
        private readonly Random _random = new();

        [Test]
        public void Generation()
        {
            var number = _random.Next();
            var int32 = new NumeralSystems.Net.Type.Base.Long(){ Value = number };
            Assert.That(number, Is.EqualTo(int32.Value));
            var fromBinary = new NumeralSystems.Net.Type.Base.Long()
            {
                Binary = int32.Binary
            };
            Assert.That(number, Is.EqualTo(fromBinary.Value));
            var fromBytes = new NumeralSystems.Net.Type.Base.Long()
            {
                Bytes = int32.Bytes
            };
            Assert.That(number, Is.EqualTo(fromBytes.Value));
            for (var i = 0; i < fromBinary.Binary.Length; i++)
            {
                Assert.That(fromBinary.Binary[i], Is.EqualTo(fromBinary.Value.GetBoolAtIndex((uint)i)));
            }
        }
        
        [Test]
        public void And()
        {
            var int32A = new NumeralSystems.Net.Type.Base.Long()
            {
                Value = _random.Next()
            };
            var int32B = new NumeralSystems.Net.Type.Base.Long()
            {
                Value = _random.Next()
            };
            Assert.That(int32A.Value & int32B.Value, Is.EqualTo(int32A.And(int32B).Value));
        }
        
        [Test]
        public void Or()
        {
            var int32A = new NumeralSystems.Net.Type.Base.Long()
            {
                Value = _random.Next()
            };
            var int32B = new NumeralSystems.Net.Type.Base.Long()
            {
                Value = _random.Next()
            };
            Assert.That(int32A.Value | int32B.Value, Is.EqualTo(int32A.Or(int32B).Value));
        }
        
        [Test]
        public void Xor()
        {
            var int32A = new NumeralSystems.Net.Type.Base.Long()
            {
                Value = _random.Next()
            };
            var int32B = new NumeralSystems.Net.Type.Base.Long()
            {
                Value = _random.Next()
            };
            Assert.That(int32A.Value ^ int32B.Value, Is.EqualTo(int32A.Xor(int32B).Value));
        }
        
        [Test]
        public void Not()
        {
            var int32 = new NumeralSystems.Net.Type.Base.Long()
            {
                Value = _random.Next()
            };
            Assert.That(~int32.Value, Is.EqualTo(int32.Not().Value));
        }
        
        [Test]
        public void Nand()
        {
            var int32A = new NumeralSystems.Net.Type.Base.Long()
            {
                Value = _random.Next()
            };
            var int32B = new NumeralSystems.Net.Type.Base.Long()
            {
                Value = _random.Next()
            };
            Assert.That(~(int32A.Value & int32B.Value), Is.EqualTo(int32A.Nand(int32B).Value));
        }
        
        [Test]
        public void ReverseAnd()
        {
            var int32A = new NumeralSystems.Net.Type.Base.Long()
            {
                Value = _random.Next()
            };
            var int32B = new NumeralSystems.Net.Type.Base.Long()
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
            var int32A = new NumeralSystems.Net.Type.Base.Long()
            {
                Value = _random.Next()
            };
            var int32B = new NumeralSystems.Net.Type.Base.Long()
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