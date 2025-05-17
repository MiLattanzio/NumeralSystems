using System;
using NumeralSystems.Net.Utils;
using NUnit.Framework;
using Polecola.Primitive;

namespace NumeralSystem.Net.NUnit.Type
{
    [TestFixture]
    public class Byte: IBaseTypeTest
    {
        private readonly Random _random = new();

        [Test]
        public void Generation()
        {
            var bytes = new byte[1];
            _random.NextBytes(bytes);
            var int32 = new NumeralSystems.Net.Type.Base.Byte(){ Value = bytes[0] };
            Assert.That(bytes[0], Is.EqualTo(int32.Value));
            var fromBinary = new NumeralSystems.Net.Type.Base.Byte()
            {
                Binary = int32.Binary
            };
            Assert.That(bytes[0], Is.EqualTo(fromBinary.Value));
            var fromBytes = new NumeralSystems.Net.Type.Base.Byte()
            {
                Bytes = int32.Bytes
            };
            Assert.That(bytes[0], Is.EqualTo(fromBytes.Value));
            for (var i = 0; i < fromBinary.Binary.Length; i++)
            {
                Assert.That(fromBinary.Binary[i], Is.EqualTo(fromBinary.Value.GetBoolAtIndex((uint)i)));
            }
        }
        
        [Test]
        public void And()
        {
            var bytes = new byte[2];
            _random.NextBytes(bytes);
            var a = bytes[0];
            var b = bytes[1];
            var int32A = new NumeralSystems.Net.Type.Base.Byte()
            {
                Value = a
            };
            var int32B = new NumeralSystems.Net.Type.Base.Byte()
            {
                Value = b
            };
            Assert.That(int32A.Value & int32B.Value, Is.EqualTo(int32A.And(int32B).Value));
        }
        
        [Test]
        public void Or()
        {
            var bytes = new byte[2];
            _random.NextBytes(bytes);
            var a = bytes[0];
            var b = bytes[1];
            var int32A = new NumeralSystems.Net.Type.Base.Byte()
            {
                Value = a
            };
            var int32B = new NumeralSystems.Net.Type.Base.Byte()
            {
                Value = b
            };
            Assert.That(int32A.Value | int32B.Value, Is.EqualTo(int32A.Or(int32B).Value));
        }
        
        [Test]
        public void Xor()
        {
            var bytes = new byte[2];
            _random.NextBytes(bytes);
            var a = bytes[0];
            var b = bytes[1];
            var int32A = new NumeralSystems.Net.Type.Base.Byte()
            {
                Value = a
            };
            var int32B = new NumeralSystems.Net.Type.Base.Byte()
            {
                Value = b
            };
            Assert.That(int32A.Value ^ int32B.Value, Is.EqualTo(int32A.Xor(int32B).Value));
        }
        
        [Test]
        public void Not()
        {
            var bytes = new byte[1];
            _random.NextBytes(bytes);
            var a = bytes[0];
            var int32 = new NumeralSystems.Net.Type.Base.Byte()
            {
                Value = a
            };
            Assert.That((byte)~a, Is.EqualTo(int32.Not().Value));
        }
        
        [Test]
        public void Nand()
        {
            var bytes = new byte[2];
            _random.NextBytes(bytes);
            var a = bytes[0];
            var b = bytes[1];
            var int32A = new NumeralSystems.Net.Type.Base.Byte()
            {
                Value = a
            };
            var int32B = new NumeralSystems.Net.Type.Base.Byte()
            {
                Value = b
            };
            Assert.That((byte)~(int32A.Value & int32B.Value), Is.EqualTo(int32A.Nand(int32B).Value));
        }
        
        [Test]
        public void ReverseAnd()
        {
            var bytes = new byte[2];
            _random.NextBytes(bytes);
            var a = bytes[0];
            var b = bytes[1];
            var int32A = new NumeralSystems.Net.Type.Base.Byte()
            {
                Value = a
            };
            var int32B = new NumeralSystems.Net.Type.Base.Byte()
            {
                Value = b
            };
            var result = int32A.And(int32B);
            var success = result.ReverseAnd(int32B, out var int32AReversed);
            Assert.IsTrue(success);
            Assert.IsTrue(int32AReversed.Contains(int32A));
        }
        
        [Test]
        public void ReverseOr()
        {
            var bytes = new byte[2];
            _random.NextBytes(bytes);
            var a = bytes[0];
            var b = bytes[1];
            var int32A = new NumeralSystems.Net.Type.Base.Byte()
            {
                Value = a
            };
            var int32B = new NumeralSystems.Net.Type.Base.Byte()
            {
                Value = b
            };
            var result = int32A.Or(int32B);
            var success = result.ReverseOr(int32B, out var int32AReversed);
            Assert.IsTrue(success);
            Assert.IsTrue(int32AReversed.Contains(int32A));
        }
        
        
    }
}